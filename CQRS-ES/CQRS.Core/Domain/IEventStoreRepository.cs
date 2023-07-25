using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public interface IEventStoreRepository
    {
        Task SaveAsync(EventModel @event);

        Task<List<EventModel>> FindByAggregareId(Guid aggregateId);

        Task<List<EventModel>> FindAllAsync();
    }
}
