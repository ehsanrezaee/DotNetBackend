using System.Data;
using System.Linq;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace ErSoftDev.Framework.BaseModel
{
    public class BaseDbContext : DbContext, IUnitOfWork
    {
        private readonly IOptions<AppSetting> _appSetting;
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private IDbContextTransaction _currentTransaction;

        public BaseDbContext(DbContextOptions options, IOptions<AppSetting> appSetting,
             IMediator mediator, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _appSetting = appSetting;
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (_appSetting?.Value.ConnectionString != null)
            //    optionsBuilder.UseSqlServer(_appSetting.Value.ConnectionString);
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entitiesAssembly = AppDomain.CurrentDomain.GetAssemblies();
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddPluralizingTableNameConvention<IHaveNotPluralized>();
            modelBuilder.SoftDeleteQueryFilter<ISoftDelete>(entitiesAssembly);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        {
                            if (entry.Properties.Any(e => e.Metadata.Name == "IsDeleted") ||
                                entry.Properties.Any(e => e.Metadata.Name == "DeletedAt") ||
                                entry.Properties.Any(e => e.Metadata.Name == "DeleterUserId"))
                            {

                                if (entry.Collections.Any() && !entry.Collections.Any(collectionEntry =>
                                        collectionEntry.EntityEntry.Metadata.GetProperties()
                                            .Any(x => x.Name == "IsDeleted")))
                                    throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.DbError);

                                var collectionCount = entry.Metadata.GetReferencingForeignKeys().Count(foreignKey =>
                                    foreignKey.DeclaringEntityType.ClrType.BaseType?.Name != nameof(ValueObject));
                                if (collectionCount != entry.Collections.Count())
                                    throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.DbError);


                                //foreach (var collection in entry.Collections)
                                //{
                                //    if(collection.)
                                //}

                                entry.State = EntityState.Modified;
                                if (entry.Properties.Any(e =>
                                        e.Metadata.Name == "DeleterUserId" &&
                                        _httpContextAccessor.UserIdClaimIdentity() is not null))
                                    entry.Property("DeleterUserId").CurrentValue =
                                        long.Parse(_httpContextAccessor.UserIdClaimIdentity()!);
                                if (entry.Properties.Any(e => e.Metadata.Name == "DeletedAt"))
                                    entry.Property("DeletedAt").CurrentValue = DateTime.Now;
                                if (entry.Properties.Any(e => e.Metadata.Name == "IsDeleted"))
                                    entry.Property("IsDeleted").CurrentValue = true;
                            }

                            break;
                        }
                    case EntityState.Modified:
                        {
                            if (entry.Properties.Any(e => e.Metadata.Name == "UpdatedAt"))
                                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                            if (entry.Properties.Any(e => e.Metadata.Name == "UpdaterUserId") &&
                                _httpContextAccessor.UserIdClaimIdentity() is not null)
                                entry.Property("UpdaterUserId").CurrentValue =
                                    long.Parse(_httpContextAccessor.UserIdClaimIdentity()!);

                            break;
                        }
                    case EntityState.Added:
                        {
                            if (entry.Properties.Any(e => e.Metadata.Name == "CreatedAt"))
                                entry.Property("CreatedAt").CurrentValue = DateTime.Now;
                            if (entry.Properties.Any(e => e.Metadata.Name == "CreatedAt") &&
                                _httpContextAccessor.UserIdClaimIdentity() is not null)
                                entry.Property("CreatorUserId").CurrentValue =
                                    long.Parse(_httpContextAccessor.UserIdClaimIdentity()!);
                            if (entry.Properties.Any(e => e.Metadata.Name == "IsDeleted"))
                                entry.Property("IsDeleted").CurrentValue = false;

                            break;
                        }
                }
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            await SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveEntitiesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                RollbackTransaction();
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.DbError);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
