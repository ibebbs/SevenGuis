using System;

namespace Cells.Common.Event
{
    public class TextChanged : IEvent
    {
        public TextChanged(Guid id, Guid causationId, Guid correlationId)
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

        public string Text { get; set; }
    }
}
