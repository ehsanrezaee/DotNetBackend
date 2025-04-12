using System.Linq.Expressions;
using ErSoftDev.Identity.Infrastructure.NoSql.Models;
using MongoDB.Driver;

namespace ErSoftDev.Identity.Infrastructure.NoSql.Repositories;

public interface IInstrumentMongoRepository
{
    Task<IMongoCollection<Instrument>> GetCollection();
    Task<IEnumerable<Instrument>> GetAll(CancellationToken cancellationToken);
    Task<Instrument> GetById(string id, CancellationToken cancellationToken);
    Task<Instrument> Get(Expression<Func<Instrument, bool>> predicate, CancellationToken cancellationToken);
    Task Create(Instrument entity);
    Task<bool> Update(Instrument entity);
    Task AddOrUpdate(Instrument entity, CancellationToken cancellationToken);
    Task<bool> Delete(string id);
}