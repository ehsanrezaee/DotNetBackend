using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAndPassword(string username, string password, CancellationToken cancellationToken);

        Task<User?> GetUser(long id);

        Task<User?> GetByRefreshToken(string refreshToken);

        Task<User?> GetUserBySecurityStampToken(string securityStampToken, CancellationToken cancellationToken);
    }
}
