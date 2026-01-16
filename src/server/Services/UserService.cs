
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    // Вспомогательный метод для работы с пользователем
    public async Task<User> FindOrCreateUser(string googleId, string email, string name, bool emailVerified, AppDbContext _context, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, cancellationToken);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                GoogleId = googleId,
                Name = name,
                Email = email,
                EmailVerified = emailVerified,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
        }
        else
        {
            // Обновляем время последнего входа
            user.LastLoginAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return user;
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
    public async Task<User?> GetUserByIdAsync(Guid userId, AppDbContext _context, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user;
    }
}