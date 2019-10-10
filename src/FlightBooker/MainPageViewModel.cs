using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlightBooker
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly Crux.Property<FlightType> _flightType;
        private readonly Crux.Property<string> _outboundText;
        private readonly Crux.Property<bool> _outboundValid;
        private readonly Crux.Property<string> _returnText;
        private readonly Crux.Property<bool> _returnValid;
        private readonly Crux.Property<bool> _returnAvailable;
        private readonly Crux.Command _book;

        private readonly IObservable<DateTime?> _outboundDate;
        private readonly IObservable<DateTime?> _returnDate;

        private IDisposable _subscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            _flightType = new Crux.Property<FlightType>(FlightType.OneWay, nameof(FlightType), args => PropertyChanged?.Invoke(this, args));
            _outboundText = new Crux.Property<string>(DateTime.Now.ToString("d"), nameof(OutboundText), args => PropertyChanged?.Invoke(this, args));
            _outboundValid = new Crux.Property<bool>(nameof(OutboundValid), args => PropertyChanged?.Invoke(this, args));
            _returnText = new Crux.Property<string>(DateTime.Now.ToString("d"), nameof(ReturnText), args => PropertyChanged?.Invoke(this, args));
            _returnValid = new Crux.Property<bool>(nameof(ReturnValid), args => PropertyChanged?.Invoke(this, args));
            _returnAvailable = new Crux.Property<bool>(false, nameof(ReturnAvailable), args => PropertyChanged?.Invoke(this, args));
            _book = new Crux.Command();

            _outboundDate = _outboundText.Select(text => DateTime.TryParseExact(text, "d", Thread.CurrentThread.CurrentUICulture, System.Globalization.DateTimeStyles.None, out DateTime date) ? (DateTime?)date : null);
            _returnDate = _returnText.Select(text => DateTime.TryParseExact(text, "d", Thread.CurrentThread.CurrentUICulture, System.Globalization.DateTimeStyles.None, out DateTime date) ? (DateTime?)date : null);
        }

        private IDisposable ShouldEnableReturnWhenFlightTypeIsReturn()
        {
            return _flightType
                .Select(flightType => flightType == FlightType.Return)
                .Subscribe(_returnAvailable);
        }

        private IDisposable ShouldSetOutboundValidWhenOutboundDateHasValue()
        {
            return _outboundDate
                .Select(date => date != null)
                .Subscribe(_outboundValid);
        }

        private IDisposable ShouldSetReturnValidWhenReturnDateHasValueOrFlightTypeIsOneWay()
        {
            return _flightType
                .Select(flightType => _returnDate.Select(date => new { FlightType = flightType, Date = date }))
                .Switch()
                .Select(tuple => tuple.FlightType == FlightType.OneWay || tuple.Date != null)
                .Subscribe(_returnValid);
        }

        private IDisposable ShouldEnableBookWhenFlightTypeIsOneWayAndOutboundDateIsValid()
        {
            return _flightType
                .Select(flightType => flightType == FlightType.OneWay ? _outboundValid : Observable.Never<bool>())
                .Switch()
                .Subscribe(_book);
        }

        private IDisposable ShouldEnableBookWhenFlightTypeIsReturnAndBothOutboundAndReturnDatesAreValid()
        {
            var datesValid = Observable.CombineLatest(_outboundDate, _returnDate).Select(dates => dates.All(date => date.HasValue) && dates[1] >= dates[0]);

            return _flightType
                .Select(flightType => flightType == FlightType.Return ? datesValid : Observable.Never<bool>())
                .Switch()
                .Subscribe(_book);
        }

        public void Activate()
        {
            _subscription = new CompositeDisposable(
                ShouldEnableReturnWhenFlightTypeIsReturn(),
                ShouldSetOutboundValidWhenOutboundDateHasValue(),
                ShouldSetReturnValidWhenReturnDateHasValueOrFlightTypeIsOneWay(),
                ShouldEnableBookWhenFlightTypeIsOneWayAndOutboundDateIsValid(),
                ShouldEnableBookWhenFlightTypeIsReturnAndBothOutboundAndReturnDatesAreValid()
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

        public FlightType FlightType
        {
            get { return _flightType.Get(); }
            set { _flightType.Set(value); }
        }

        public string OutboundText
        {
            get { return _outboundText.Get(); }
            set { _outboundText.Set(value); }
        }

        public bool OutboundValid
        {
            get { return _outboundValid.Get(); }
        }

        public string ReturnText
        {
            get { return _returnText.Get(); }
            set { _returnText.Set(value); }
        }

        public bool ReturnValid
        {
            get { return _returnValid.Get(); }
        }

        public bool ReturnAvailable
        {
            get { return _returnAvailable.Get(); }
        }

        public ICommand Book
        {
            get { return _book; }
        }
    }
}
