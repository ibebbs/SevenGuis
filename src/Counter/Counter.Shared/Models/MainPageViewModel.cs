using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Counter.Shared.Models
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Crux.Property<int> _counter;
        private readonly Crux.Command _increment;

        private IDisposable _subscription;

        public MainPageViewModel()
        {
            _counter = new Crux.Property<int>(nameof(Counter), args => PropertyChanged?.Invoke(this, args));
            _increment = new Crux.Command(true);
        }

        private IDisposable CounterShouldBeIncrementedWhenIncrementIsInvoked()
        {
            return _counter
                .Select(value => _increment.Select(_ => value + 1))
                .Switch()
                .Subscribe(_counter);
        }

        public void Activate()
        {
            _subscription = new CompositeDisposable(
                CounterShouldBeIncrementedWhenIncrementIsInvoked()
            );
        }

        public void Deativate()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }

        public ICommand Increment
        {
            get { return _increment; }
        }

        public int Counter
        {
            get { return _counter.Get(); }
        }
    }
}
