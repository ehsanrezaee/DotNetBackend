using System.Linq.Expressions;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.Configuration;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;

namespace ErSoftDev.Identity.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository, ITransientDependency
    {
        private readonly IdentityDbContext _identityDbContext;
        public IUnitOfWork UnitOfWork => _identityDbContext;

        public RoleRepository(IdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task Add(Role tObject, CancellationToken cancellationToken)
        {
            await _identityDbContext.Roles.AddAsync(tObject, cancellationToken);
        }

        public async Task<Role?> Get(long id, CancellationToken cancellationToken)
        {
            return await _identityDbContext.Roles.FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
        }

        public async Task<Role?> Get(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _identityDbContext.Roles.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<List<Role>?> GetList(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _identityDbContext.Roles.Where(predicate).ToListAsync(cancellationToken);
        }
    }
}
