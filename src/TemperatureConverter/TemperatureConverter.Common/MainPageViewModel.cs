using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace TemperatureConverter.Common
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly Crux.Property<int> _celcius;
        private readonly Crux.Property<int> _fahrenheit;

        private IDisposable _subscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            _celcius = new Crux.Property<int>(nameof(Celcius), args => PropertyChanged?.Invoke(this, args));
            _fahrenheit = new Crux.Property<int>(nameof(Fahrenheit), args => PropertyChanged?.Invoke(this, args));
        }

        private IDisposable ShouldUpdateFahrenheitWhenCelciusIsChanged()
        {
            return _celcius
                .DistinctUntilChanged()
                .Select(Domain.ConvertCelciusToFahrenheit)
                .Subscribe(_fahrenheit);
        }

        private IDisposable ShouldUpdateCelciusWhenFahrenheitIsChanged()
        {
            return _fahrenheit
                .DistinctUntilChanged()
                .Select(Domain.ConvertFahrenheitToCelcius)
                .Subscribe(_celcius);
        }

        public void Activate()
        {
            _subscription = new CompositeDisposable(
                ShouldUpdateFahrenheitWhenCelciusIsChanged(),
                ShouldUpdateCelciusWhenFahrenheitIsChanged()
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

        public int Celcius
        {
            get { return _celcius.Get(); }
            set { _celcius.Set(value); }
        }

        public int Fahrenheit
        {
            get { return _fahrenheit.Get(); }
            set { _fahrenheit.Set(value); }
        }
    }
}
