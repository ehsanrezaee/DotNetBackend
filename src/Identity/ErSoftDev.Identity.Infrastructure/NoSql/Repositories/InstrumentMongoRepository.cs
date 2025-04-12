using ErSoftDev.Framework.Configuration;
using ErSoftDev.Framework.Mongo;
using ErSoftDev.Identity.Infrastructure.NoSql.Models;

namespace ErSoftDev.Identity.Infrastructure.NoSql.Repositories;

public class InstrumentMongoRepository : MongoRepository<Instrument, long>, ITransientDependency, IInstrumentMongoRepository
{
    public InstrumentMongoRepository(BaseMongoDbContext context) : base(context)
    {
    }
}