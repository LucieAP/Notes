
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class UserService
{
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
    public async Task<User?> GetuserByIdAsync(Guid userId, AppDbContext _context, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user;
    }
}