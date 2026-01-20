
using System.Security.Claims;

namespace server.Interfaces.Services
{
    public interface IUserService
    {
        public Task<User> FindOrCreateUser(GoogleUserInfo userInfo, CancellationToken cancellationToken = default);
        public Guid GetUserId(ClaimsPrincipal user);
        public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}