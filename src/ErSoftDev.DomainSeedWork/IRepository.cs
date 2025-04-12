using System.Linq.Expressions;
using static System.Formats.Asn1.AsnWriter;

namespace ErSoftDev.DomainSeedWork
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        public IUnitOfWork UnitOfWork { get; }

        Task Add(T tObject, CancellationToken cancellationToken);

        Task<T?> Get(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        Task<List<T>?> GetList(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        void Delete(T tObject);
    }
}
