using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Timer.Common
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public static readonly long MinTickValue = TimeSpan.FromSeconds(10).Ticks;
        public static readonly long MaxTickValue = TimeSpan.FromSeconds(60).Ticks;
        public static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(100);

        private readonly IScheduler _scheduler;
        private readonly Crux.Property<long> _max;
        private readonly Crux.Property<long> _elapsed;
        private readonly Crux.Command _reset;

        private IDisposable _subscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel(IScheduler scheduler)
        {
            _scheduler = scheduler;
            _max = new Crux.Property<long>(TimeSpan.FromSeconds(30).Ticks, nameof(MaxTicks), args => PropertyChanged?.Invoke(this, args));
            _elapsed = new Crux.Property<long>(0, nameof(ElapsedTicks), args => PropertyChanged?.Invoke(this, args));
            _reset = new Crux.Command(true);
        }

        private IDisposable ShouldIncrementElapsedUntilResetOrEqualToMax()
        {
            return _reset
                .StartWith((object)null)
                .Select(_ => Observable
                    .Interval(Interval)
                    .WithLatestFrom(_max, (interval, max) => max)
                    .Scan((long)0, (acc, max) => acc + Interval.Ticks >= max ? max : acc + Interval.Ticks))
                .Switch()
                .ObserveOn(_scheduler)
                .Subscribe(_elapsed);
        }

        public void Activate()
        {
            _subscription = new CompositeDisposable(
                ShouldIncrementElapsedUntilResetOrEqualToMax()
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

        public long MaxTicks 
        {
            get { return _max.Get(); }
            set { _max.Set(value); }
        }

        public long ElapsedTicks 
        {
            get { return _elapsed.Get(); }
        }

        public ICommand Reset 
        {
            get { return _reset; }
        }
    }
}
