using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace CircleDrawer
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly Crux.Property<IEnumerable<Circle>> _circles;
        private readonly Crux.Property<Circle> _selected;
        private readonly Crux.Command _adjustDiameter;
        private readonly Crux.Command _undo;
        private readonly Crux.Command _redo;

        private IDisposable _behaviours;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            _circles = new Crux.Property<IEnumerable<Circle>>(nameof(Circles), args => PropertyChanged?.Invoke(this, args));
            _selected = new Crux.Property<Circle>(nameof(Selected), args => PropertyChanged?.Invoke(this, args));
            _adjustDiameter = new Crux.Command(true);
            _undo = new Crux.Command();
            _redo = new Crux.Command();
        }

        private IObservable<Domain.ICommand> Commands(IObservable<Point> emptyAreaClicked, IObservable<Unit> adjustDiameterDialogClosed)
        {
            var addCircleCommand = emptyAreaClicked
                .Select(point => new Domain.Command.AddCircle(new Circle { Origin = point, Diameter = Domain.Logic.InitialDiameter }))
                .Cast<Domain.ICommand>();

            var changeDiameterCommand = adjustDiameterDialogClosed
                .WithLatestFrom(_selected, (unit, circle) => new Domain.Command.ChangeDiameter(circle, circle.Diameter))
                .Cast<Domain.ICommand>();

            var undoCommand = _undo
                .Select(_ => new Domain.Command.Undo())
                .Cast<Domain.ICommand>();

            var redoCommand = _redo
                .Select(_ => new Domain.Command.Redo())
                .Cast<Domain.ICommand>();

            return Observable.Merge(
                addCircleCommand,
                changeDiameterCommand,
                undoCommand,
                redoCommand
            );
        }

        private IDisposable ShouldPopulateCirclesFromState(IObservable<Domain.IState> state)
        {
            return state
                .Select(s => s.Circles)
                .Subscribe(_circles);
        }

        private IDisposable ShouldEnableOrDisableRedoBasedOnState(IObservable<Domain.IState> state)
        {
            return state
                .Select(s => s.CanUndo)
                .Subscribe(_undo);
        }

        private IDisposable ShouldEnableOrDisableUndoBasedOnState(IObservable<Domain.IState> state)
        {
            return state
                .Select(s => s.CanRedo)
                .Subscribe(_redo);
        }

        private IDisposable ShouldSetSelectedFromState(IObservable<Domain.IState> state)
        {
            return state
                .Select(s => s.Selected)
                .Subscribe(_selected);
        }

        private IDisposable ShouldShowAdjustDiameterDialogWhenAdjustDiameterInvoked(IObserver<Unit> showAdjustDiameterDialog)
        {
            return _adjustDiameter
                .Select(_ => Unit.Default)
                .Subscribe(showAdjustDiameterDialog);
        }

        public void Activate(IObservable<Point> emptyAreaClicked, IObserver<Unit> showAdjustDiameterDialog, IObservable<Unit> adjustDiameterDialogClosed)
        {
            var commands = Commands(emptyAreaClicked, adjustDiameterDialogClosed);

            var state = Domain.Logic
                .Observable(commands)
                .Publish()
                .RefCount();

            _behaviours = new CompositeDisposable(
                ShouldPopulateCirclesFromState(state),
                ShouldEnableOrDisableUndoBasedOnState(state),
                ShouldEnableOrDisableRedoBasedOnState(state),
                ShouldSetSelectedFromState(state),
                ShouldShowAdjustDiameterDialogWhenAdjustDiameterInvoked(showAdjustDiameterDialog)
            );
        }

        public void Deactivate()
        {
            if (_behaviours != null)
            {
                _behaviours.Dispose();
                _behaviours = null;
            }
        }

        public IEnumerable<Circle> Circles => _circles.Get();

        public Circle Selected
        {
            get { return _selected.Get(); }
            set { _selected.Set(value); }
        }

        public ICommand AdjustDiameter => _adjustDiameter;

        public ICommand Undo => _undo;

        public ICommand Redo => _redo;
    }
}
