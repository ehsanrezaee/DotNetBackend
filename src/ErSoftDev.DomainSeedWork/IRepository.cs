using System.Linq.Expressions;

namespace ErSoftDev.DomainSeedWork
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        public IUnitOfWork UnitOfWork { get; }

        Task Add(T user);

        Task<T?> Get(long id, CancellationToken cancellationToken);

        Task<List<T>> GetList(Expression<Func<T, bool>> predicate);
    }
}
