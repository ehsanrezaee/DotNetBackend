using System.Linq.Expressions;

namespace ErSoftDev.Framework.Mongo;

public interface IMongoRepository<T>
{
    public Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken);
    public Task<T> GetById(string id, CancellationToken cancellationToken);
    public Task<T> Get(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
    public Task Create(T entity);
    public Task<bool> Update(T entity);
    public Task AddOrUpdate(T entity, CancellationToken cancellationToken);
    public Task<bool> Delete(string id);
}