using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ErSoftDev.DomainSeedWork
{
    public interface IHaveNotPluralized
    {

    }
    public interface IEntity
    {

    }

    public interface ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public abstract class BaseEntity : DomainEvent, IEntity
    {
        public long CreatorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? UpdaterUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public abstract class BaseEntity<TKey> : BaseEntity
    {
        [Key]
        public TKey? Id { get; set; }
    }

    public abstract class DomainEvent
    {
        private List<INotification>? _domainEvents;
        public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
