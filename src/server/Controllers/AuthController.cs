using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // Перенаправляет пользователя на страницу входа Google. 
        // После успешной аутентификации Google вернёт пользователя на URL, указанный в RedirectUri.
        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = "/api/auth/google-callback";
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(CancellationToken cancellationToken = default)
        {
            // Шаг 1: Получаем данные от Google
            var googleData = await HttpContext.AuthenticateAsync("ExternalCookies");

            if (!googleData.Succeeded)
            {
                return Unauthorized("Ошибка входа через Google.");
            }

            // Шаг 2: Извлекаем информацию о пользователе

            var claims = googleData.Principal.Claims;

            var userInfo = new GoogleUserInfo
            {
                GoogleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                Picture = claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                EmailVerified = bool.TryParse(claims.FirstOrDefault(c => c.Type == "email_verified")?.Value, out var emailVerified) && emailVerified
            };


            if (string.IsNullOrEmpty(userInfo.GoogleId) || string.IsNullOrEmpty(userInfo.Email))
            {
                return BadRequest("Не удалось получить данные пользователя.");
            }

             // Шаг 3: Находим или создаём пользователя
            var user = await FindOrCreateUser(userInfo.GoogleId, userInfo.Email, userInfo.Name, userInfo.EmailVerified, cancellationToken);

            // Шаг 4: Генерируем токен
            var jwtToken = _jwtService.GenerateJwtToken(new JwtUser 
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });

            // Возвращаем JWT (в реальном проекте можно redirect на frontend с токеном в query или cookie)
            // Для API просто возвращаем JSON
            return Ok(new { Token = jwtToken });

            // TO-DO: Redirect на React с токеном
            // return Redirect($"http://localhost:3000/callback?token={jwtToken}");
        }

        // Вспомогательный метод для работы с пользователем
        private async Task<User> FindOrCreateUser(string googleId, string email, string name, bool emailVerified, CancellationToken cancellationToken)
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
                user.LastLoginAt = DateTime.Now;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}