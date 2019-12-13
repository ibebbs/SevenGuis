using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Crud
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly MVx.Observable.Property<string> _filter;
        private readonly MVx.Observable.Property<string> _name;
        private readonly MVx.Observable.Property<string> _surname;
        private readonly MVx.Observable.Property<IEnumerable<FullName>> _names;
        private readonly MVx.Observable.Property<FullName?> _selected;
        private readonly MVx.Observable.Command _create;
        private readonly MVx.Observable.Command _update;
        private readonly MVx.Observable.Command _delete;

        private readonly BehaviorSubject<IEnumerable<FullName>> _allNames;
        private readonly IObservable<FullName?> _fullName;

        private IDisposable _subscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {

            _filter = new MVx.Observable.Property<string>(nameof(Filter), args => PropertyChanged?.Invoke(this, args));
            _name = new MVx.Observable.Property<string>(nameof(Name), args => PropertyChanged?.Invoke(this, args));
            _surname = new MVx.Observable.Property<string>(nameof(Surname), args => PropertyChanged?.Invoke(this, args));
            _names = new MVx.Observable.Property<IEnumerable<FullName>>(nameof(Names), args => PropertyChanged?.Invoke(this, args));
            _selected = new MVx.Observable.Property<FullName?>(null, nameof(Selected), args => PropertyChanged?.Invoke(this, args));
            _create = new MVx.Observable.Command();
            _update = new MVx.Observable.Command();
            _delete = new MVx.Observable.Command();

            _allNames = new BehaviorSubject<IEnumerable<FullName>>(Enumerable.Empty<FullName>());
            _fullName = Observable
                .CombineLatest(_name, _surname)
                .Select(parts => parts.Any(part => string.IsNullOrEmpty(part)) ? (FullName?)null : new FullName { Name = parts[0], Surname = parts[1] })
                .Publish()
                .RefCount();
        }

        private IDisposable ShouldEnableUpdateWhenSelected()
        {
            return _selected
                .Select(selected => selected.HasValue)
                .Subscribe(_update);
        }

        private IDisposable ShouldEnableDeleteWhenSelected()
        {
            return _selected
                .Select(selected => selected.HasValue)
                .Subscribe(_delete);
        }

        private IDisposable ShouldEnableCreateWhenNameAndSurnameArePopulated()
        {
            return _fullName
                .Select(name => name.HasValue)
                .Subscribe(_create);
        }

        private IDisposable ShouldAddFullNameWhenCreateInvoked()
        {
            var allNames = _create
                .WithLatestFrom(_fullName, (obj, name) => name)
                .Where(fullName => fullName.HasValue)
                .WithLatestFrom(_allNames, (current, all) => all.Concat(new[] { current.Value }))
                .Publish()
                .RefCount();

            return new CompositeDisposable(
                allNames.Subscribe(_allNames),
                allNames.Select(_ => (FullName?)null).Subscribe(_selected)
            );
        }

        private IEnumerable<FullName> ApplyFilter(string filter, IEnumerable<FullName> fullNames)
        {
            return string.IsNullOrEmpty(filter)
                ? fullNames
                : fullNames.Where(fn => fn.Surname.StartsWith(filter)).ToArray();
        }

        private IDisposable ShouldPopulateNamesWithFilteredNames()
        {
            return _allNames
                .Select(allNames => _filter.Select(filter => ApplyFilter(filter, allNames)))
                .Switch()
                .Subscribe(_names);
        }

        private IDisposable ShouldUpdateFullNameWhenUpdatedInvoked()
        {
            return _update
                .WithLatestFrom(_selected, (obj, selected) => selected)
                .WithLatestFrom(_fullName, (selected, fullName) => new { Selected = selected, FullName = fullName })
                .Where(tuple => tuple.Selected.HasValue && tuple.FullName.HasValue)
                .WithLatestFrom(_allNames, (tuple, allNames) => allNames
                    .TakeWhile(name => !name.Equals(tuple.Selected.Value))
                    .Concat(new[] { tuple.FullName.Value })
                    .Concat(allNames.SkipWhile(name => !name.Equals(tuple.Selected)).Skip(1))
                    .ToArray())
                .Subscribe(_allNames);
        }

        private IDisposable ShouldRemoveFullNameWhenDeleteInvoked()
        {
            return _delete
                .WithLatestFrom(_selected, (obj, selected) => selected)
                .Where(selected => selected.HasValue)
                .WithLatestFrom(_allNames, (selected, allNames) => allNames.Except(new[] { selected.Value }).ToArray())
                .Subscribe(_allNames);
        }

        private IDisposable ShouldPopulateNameWhenFullNameSelected()
        {
            return _selected
                .Select(selected => selected?.Name ?? string.Empty)
                .Subscribe(_name);
        }

        private IDisposable ShouldPopulateSurnameWhenFullNameSelected()
        {
            return _selected
                .Select(selected => selected?.Surname ?? string.Empty)
                .Subscribe(_surname);
        }

        public void Activate()
        {
            _subscription = new CompositeDisposable(
                ShouldEnableUpdateWhenSelected(),
                ShouldEnableDeleteWhenSelected(),
                ShouldEnableCreateWhenNameAndSurnameArePopulated(),
                ShouldPopulateNamesWithFilteredNames(),
                ShouldAddFullNameWhenCreateInvoked(),
                ShouldUpdateFullNameWhenUpdatedInvoked(),
                ShouldRemoveFullNameWhenDeleteInvoked(),
                ShouldPopulateNameWhenFullNameSelected(),
                ShouldPopulateSurnameWhenFullNameSelected()
            );
        }

        public void Deactivate()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }

        public string Filter
        {
            get { return _filter.Get(); }
            set { _filter.Set(value); }
        }

        public string Name
        {
            get { return _name.Get(); }
            set { _name.Set(value); }
        }

        public string Surname
        {
            get { return _surname.Get(); }
            set { _surname.Set(value); }
        }

        public IEnumerable<FullName> Names 
        {
            get { return _names.Get(); }
        }

        public FullName? Selected 
        {
            get { return _selected.Get(); }
            set { _selected.Set(value); }
        }

        public ICommand Create
        {
            get { return _create; }
        }

        public ICommand Update 
        {
            get { return _update; }
        }

        public ICommand Delete 
        {
            get { return _delete; }
        }
    }
}
