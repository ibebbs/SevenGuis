using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Cells.Common
{
    public class CellViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly MVx.Observable.IBus _bus;
        private readonly MVx.Observable.Property<object> _content;
        private readonly IScheduler _scheduler;
        private readonly MVx.Observable.Property<string> _text;

        private IDisposable _subscription;

        public CellViewModel(MVx.Observable.IBus bus, int row, char column, IScheduler scheduler)
        {
            _bus = bus;
            _text = new MVx.Observable.Property<string>(nameof(Text), args => PropertyChanged?.Invoke(this, args));
            _content = new MVx.Observable.Property<object>(nameof(Content), args => PropertyChanged?.Invoke(this, args));
            _scheduler = scheduler;

            Row = row;
            Column = column;

            _subscription = new CompositeDisposable(
                ShouldUpdateContentWhenContentChangedReceived(),
                SholdPublishTextChangedWhenTextChanged()
            );
        }

        private IDisposable SholdPublishTextChangedWhenTextChanged()
        {
            return _text
                .Skip(1) // Skip initial value
                .DistinctUntilChanged()
                .Select(text => new Event.TextChanged(Guid.NewGuid(), Guid.Empty, Guid.Empty) { Row = Row, Column = Column, Text = text })
                .Subscribe(_bus.Publish);
        }

        private IDisposable ShouldUpdateContentWhenContentChangedReceived()
        {
            return _bus
                .GetEvent<Event.ContentChanged>()
                .Where(@event => @event.Row == Row && @event.Column == Column)
                .Select(@event => @event.Content)
                .ObserveOn(_scheduler)
                .Subscribe(_content);
        }

        public void Dispose()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Row { get; }

        public char Column { get; }

        public string Text
        {
            get { return _text.Get(); }
            set { _text.Set(value); }
        }

        public object Content
        {
            get { return _content.Get(); }
        }
    }
}
