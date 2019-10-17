using System;

namespace Cells.Common
{
    public interface IEvent
    {
        Guid Id { get; }
        Guid CausationId { get; }
        Guid CorrelationId { get; }
    }
}
