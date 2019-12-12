using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;

namespace FlightBooker.Common
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IScheduler _scheduler;
        private readonly MVx.Observable.Property<FlightType> _flightType;
        private readonly MVx.Observable.Property<string> _outboundText;
        private readonly MVx.Observable.Property<bool> _outboundValid;
        private readonly MVx.Observable.Property<string> _returnText;
        private readonly MVx.Observable.Property<bool> _returnValid;
        private readonly MVx.Observable.Property<bool> _returnAvailable;
        private readonly MVx.Observable.Command _book;
        private readonly MVx.Observable.Property<string> _message;

        private readonly IObservable<DateTime?> _outboundDate;
        private readonly IObservable<DateTime?> _returnDate;

        private IDisposable _subscription;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel(IScheduler scheduler)
        {
            _scheduler = scheduler;
            _flightType = new MVx.Observable.Property<FlightType>(FlightType.OneWay, nameof(FlightType), args => PropertyChanged?.Invoke(this, args));
            _outboundText = new MVx.Observable.Property<string>(DateTime.Now.ToString("d"), nameof(OutboundText), args => PropertyChanged?.Invoke(this, args));
            _outboundValid = new MVx.Observable.Property<bool>(nameof(OutboundValid), args => PropertyChanged?.Invoke(this, args));
            _returnText = new MVx.Observable.Property<string>(DateTime.Now.ToString("d"), nameof(ReturnText), args => PropertyChanged?.Invoke(this, args));
            _returnValid = new MVx.Observable.Property<bool>(nameof(ReturnValid), args => PropertyChanged?.Invoke(this, args));
            _returnAvailable = new MVx.Observable.Property<bool>(false, nameof(ReturnAvailable), args => PropertyChanged?.Invoke(this, args));
            _message = new MVx.Observable.Property<string>(string.Empty, nameof(Message), args => PropertyChanged?.Invoke(this, args));
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
            return Observable
                .CombineLatest(_flightType, _returnDate, (flightType, returnDate) => (FlightType: flightType, ReturnDate: returnDate))
                .Select(tuple => tuple.FlightType == FlightType.OneWay || tuple.ReturnDate.HasValue)
                .Subscribe(_returnValid);
        }

        private bool AreValidDatesForFlightType(FlightType flightType, DateTime? outboundDate, DateTime? returnDate)
        {
            return (flightType == FlightType.OneWay && outboundDate.HasValue)
                || (outboundDate.HasValue && returnDate.HasValue && returnDate.Value >= outboundDate.Value);
        }

        private IDisposable ShouldEnableBookWhenDatesAreValidForTheSelectedFlightType()
        {
            return Observable
                .CombineLatest(_flightType, _outboundDate, _returnDate, AreValidDatesForFlightType)
                .Subscribe(_book);
        }

        private string MessageText(FlightType flightType, DateTime? outbound, DateTime? returning)
        {
            switch (flightType)
            {
                case FlightType.OneWay when outbound.HasValue: 
                    return $"You have booked a one-way flight on {outbound.Value.ToString("d")}.";
                case FlightType.Return when outbound.HasValue && returning.HasValue:
                    return $"You have booked a return flight leaving on {outbound.Value.ToString("d")} and returning on {returning.Value.ToString("d")}.";
                default: return string.Empty;
            }
        }

        private IDisposable ShouldDisplayMessageWhenBookInvoked()
        {
            var message = Observable.CombineLatest(_flightType, _outboundDate, _returnDate, MessageText);

            return _book
                .WithLatestFrom(message, (_, m) => m)
                .Select(m => string.IsNullOrEmpty(m)
                    ? Observable.Return(string.Empty)
                    : Observable.Merge(Observable.Return(m), Observable.Return(string.Empty).Delay(TimeSpan.FromSeconds(5), _scheduler)))
                .Switch()
                .Subscribe(_message);
        }

        public void Activate()
        {
            _subscription = new CompositeDisposable(
                ShouldEnableReturnWhenFlightTypeIsReturn(),
                ShouldSetOutboundValidWhenOutboundDateHasValue(),
                ShouldSetReturnValidWhenReturnDateHasValueOrFlightTypeIsOneWay(),
                ShouldEnableBookWhenDatesAreValidForTheSelectedFlightType(),
                ShouldDisplayMessageWhenBookInvoked()
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

        public string Message
        {
            get { return _message.Get(); }
        }
    }
}
