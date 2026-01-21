
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using server.Interfaces;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    // Получение идентификатора пользователя
    public Guid GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User не найден.");
        }

        return userId;
    }
    
    // Получение пользователя по его идентификатору
    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user;
    }

    // Поиск или создание пользователя
    public async Task<User> FindOrCreateUser(GoogleUserInfo userInfo, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == userInfo.GoogleId, cancellationToken);

        // Создаем нового пользователя, если его не существует
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                GoogleId = userInfo.GoogleId,
                Name = userInfo.Name,
                Email = userInfo.Email,
                Picture = userInfo.Picture,
                EmailVerified = userInfo.EmailVerified,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
        }
        else
        {
            // Если пользователь изменил данные, меняем их в БД на новые
            if (user.Name != userInfo.Name)
            {
                user.Name = userInfo.Name;
            }
            if (user.Picture != userInfo.Picture)
            {
                user.Picture = userInfo.Picture;
            }
            if (user.Email != userInfo.Email)
            {
                user.Email = userInfo.Email;
            }
            if (user.EmailVerified != userInfo.EmailVerified)
            {
                user.EmailVerified = userInfo.EmailVerified;
            }

            // Обновляем время последнего входа
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
}