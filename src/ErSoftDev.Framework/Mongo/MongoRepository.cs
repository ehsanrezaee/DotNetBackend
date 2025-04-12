using System.Linq.Expressions;
using ErSoftDev.DomainSeedWork;
using MongoDB.Driver;

namespace ErSoftDev.Framework.Mongo
{
    public class MongoRepository<T, TK> : IMongoRepository<T> where T : BaseEntity<TK>

    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(BaseMongoDbContext context)
        {
            _collection = context.GetCollection<T>();
        }

        public Task<IMongoCollection<T>> GetCollection()
        {
            return Task.FromResult(_collection);
        }

        public async Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<T> GetById(string id, CancellationToken cancellationToken)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task Create(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<bool> Update(T entity)
        {
            var result = await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", entity.Id), entity);
            return result.ModifiedCount > 0;
        }

        public async Task AddOrUpdate(T entity, CancellationToken cancellationToken)
        {
            var entityInfo = await _collection.Find(Builders<T>.Filter.Eq("Id", entity.Id)).FirstOrDefaultAsync(cancellationToken);
            if (entityInfo != null)
                await Update(entity);
            else
                await Create(entity);
        }

        public async Task<bool> Delete(string id)
        {
            var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
            return result.DeletedCount > 0;
        }
    }
}
