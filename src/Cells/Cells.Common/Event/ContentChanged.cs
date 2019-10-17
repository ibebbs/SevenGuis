using System;

namespace Cells.Common.Event
{
    public class ContentChanged : IEvent
    {
        public ContentChanged(Guid id, Guid causationId, Guid correlationId)
        {
            Id = id;
            CausationId = causationId;
            CorrelationId = correlationId;
        }

        public Guid Id { get; }

        public Guid CausationId { get; }

        public Guid CorrelationId { get; }

        public int Row { get; set; }

        public char Column { get; set; }

        public object Content { get; set; }
    }
}
