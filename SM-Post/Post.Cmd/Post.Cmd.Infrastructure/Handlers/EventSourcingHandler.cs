﻿using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
    public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
    {
        private readonly IEventStore _eventStore;

        public EventSourcingHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
        {
            var aggregate = new PostAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any()) return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(e => e.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregateRoot)
        {
            await _eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncomittedChanges(), aggregateRoot.Version);
            aggregateRoot.MarkChangesAsComitted();
        }
    }
}
