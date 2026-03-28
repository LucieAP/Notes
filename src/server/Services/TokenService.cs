using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Interfaces;

public class TokenService : ITokenService
{
    private readonly AppDbContext _context;

    public TokenService(AppDbContext context)
    {
        _context = context;
    }

    public string GenerateJwtToken(JwtUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),  // Время жизни токена
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashedToken = HashToken(rawToken);

        var refreshToken = new RefreshToken 
        {
            Id = Guid.NewGuid(),
            HashedToken = hashedToken,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false,
        };

        _context.Add(refreshToken);
        await _context.SaveChangesAsync();
        
        return rawToken;
    }

    public async Task<(string jwtToken, string refreshToken)?> RefreshAsync(string rawToken)
    {
        var hashedToken = HashToken(rawToken);

        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.HashedToken == hashedToken);

        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return null; // невалидный токен
        }

        // Rotation: инвалидируем старый, выдаём новый
        storedToken.IsRevoked = true;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Одновременное использование одного refresh-токена (replay)
            return null;
        }

        var newJwtToken = GenerateJwtToken(new JwtUser
        {
            Id = storedToken.User.Id,
            Name = storedToken.User.Name,
            Email = storedToken.User.Email
        });
        var newRefreshToken = await GenerateRefreshTokenAsync(storedToken.UserId);
        
        return (newJwtToken, newRefreshToken);
    }

    public async Task<bool> RevokeAsync(string rawToken)
    {
        var hashedToken = HashToken(rawToken);

        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.HashedToken == hashedToken);

        if (storedToken is null) return false;

        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public string HashToken(string token) {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token)); // Encoding.UTF8.GetBytes - всегда один и тот же byte[]
        return Convert.ToBase64String(bytes);
    }
}
