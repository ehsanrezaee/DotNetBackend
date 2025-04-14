using System.Linq.Expressions;
using ErSoftDev.Framework.Mongo;
using ErSoftDev.Identity.Infrastructure.NoSql.Models;
using MongoDB.Driver;

namespace ErSoftDev.Identity.Infrastructure.NoSql.Repositories;

public interface IInstrumentMongoRepository : IMongoRepository<Instrument>
{

}