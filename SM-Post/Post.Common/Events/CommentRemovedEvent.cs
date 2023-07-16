using CQRS.Core.Events;

namespace Post.Common.Events
{
    public class CommentRemovedEvent : BaseEvent
    {
        public CommentRemovedEvent() : base(nameof(PostCreatedEvent))
        {    
        }

        public Guid CommentId { get; set; }
    }
}
