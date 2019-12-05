using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;

namespace FlightBooker.Common
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly MVx.Observable.Property<FlightType> _flightType;
        private readonly MVx.Observable.Property<string> _outboundText;
        private readonly MVx.Observable.Property<bool> _outboundValid;
        private readonly MVx.Observable.Property<string> _returnText;
        private readonly MVx.Observable.Property<bool> _returnValid;
        private readonly MVx.Observable.Property<bool> _returnAvailable;
        private readonly MVx.Observable.Command _book;

        private readonly IObservable<DateTime?> _outboundDate;
        private readonly IObservable<DateTime?> _returnDate;

        private IDisposable _subscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            _flightType = new MVx.Observable.Property<FlightType>(FlightType.OneWay, nameof(FlightType), args => PropertyChanged?.Invoke(this, args));
            _outboundText = new MVx.Observable.Property<string>(DateTime.Now.ToString("d"), nameof(OutboundText), args => PropertyChanged?.Invoke(this, args));
            _outboundValid = new MVx.Observable.Property<bool>(nameof(OutboundValid), args => PropertyChanged?.Invoke(this, args));
            _returnText = new MVx.Observable.Property<string>(DateTime.Now.ToString("d"), nameof(ReturnText), args => PropertyChanged?.Invoke(this, args));
            _returnValid = new MVx.Observable.Property<bool>(nameof(ReturnValid), args => PropertyChanged?.Invoke(this, args));
            _returnAvailable = new MVx.Observable.Property<bool>(false, nameof(ReturnAvailable), args => PropertyChanged?.Invoke(this, args));
            _book = new MVx.Observable.Command();

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
