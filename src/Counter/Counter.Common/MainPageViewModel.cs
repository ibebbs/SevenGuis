﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Counter.Common
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly MVx.Observable.Property<int> _counter;
        private readonly MVx.Observable.Command _increment;

        private IDisposable _subscription;

        public MainPageViewModel()
        {
            _counter = new MVx.Observable.Property<int>(nameof(Counter), args => PropertyChanged?.Invoke(this, args));
            _increment = new MVx.Observable.Command(true);
        }

        private IDisposable CounterShouldBeIncrementedWhenIncrementIsInvoked()
        {
            return _increment
                .Scan(0, (value, _) => value + 1)
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

        public ICommand Increment => _increment;

        public int Counter => _counter.Get();
    }
}
