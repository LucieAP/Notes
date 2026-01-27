using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Interfaces;

namespace server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AuthController(IJwtService jwtService, IUserService userService)
        {
            _jwtService = jwtService;
            _userService = userService;
        }

        // Перенаправляет пользователя на страницу входа Google. 
        // После успешной аутентификации Google вернёт пользователя на URL, указанный в RedirectUri.
        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/api/auth/signin-google";
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback(CancellationToken cancellationToken = default)
        {
            // Шаг 1: Получаем данные от Google
            // Устанавливает аутентификацию через SignInScheme
            var googleData = await HttpContext.AuthenticateAsync("ExternalCookies");

            if (!googleData.Succeeded)
            {
                var errorMessage = googleData.Failure?.Message ?? "Неизвестная ошибка";
                Console.WriteLine($"Ошибка аутентификации: {errorMessage}");
                if (googleData.Failure != null)
                {
                    Console.WriteLine($"Детали ошибки: {googleData.Failure}");
                }
                return Unauthorized($"Ошибка входа через Google: {errorMessage}");
            }

            // Шаг 2: Удаляем временные куки после успешной аутентификации
            await HttpContext.SignOutAsync("ExternalCookies");

            // Шаг 3: Извлекаем информацию о пользователе

            var claims = googleData.Principal.Claims;

            // foreach (var claim in claims)
            // {
            //     Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            // }

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

             // Шаг 4: Находим или создаём пользователя
            var user = await _userService.FindOrCreateUser(userInfo, cancellationToken);

            // Шаг 5: Генерируем токен
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

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _userService.GetUserId(User);

            if (string.IsNullOrEmpty(currentUserId.ToString()))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(currentUserId, cancellationToken);

            if (user == null)
            {
                return NotFound(new { error = "User not found" }); 
            }

            return Ok(new GetUserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Picture = user.Picture,
                EmailVerified = user.EmailVerified,
                LastLoginAt = user.LastLoginAt
            });
        }
    }
}