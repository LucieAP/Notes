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
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public AuthController(
            ITokenService tokenService,
            IUserService userService,
            IWebHostEnvironment environment)
        {
            _tokenService = tokenService;
            _userService = userService;
            _environment = environment;
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

            // Шаг 5: Генерируем access-токен
            var jwtToken = _tokenService.GenerateJwtToken(new JwtUser 
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });

            // Шаг 6: Генерируем refresh-токен
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id);

            // Шаг 7: Устанавливаем в куки refresh-токен
            SetRefreshTokenCookie(refreshToken);

            // Возвращаем JWT
            // Для API просто возвращаем JSON
            // return Ok(new { Token = jwtToken });

            // Redirect
            return Redirect($"http://localhost:5173/");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var rawToken = Request.Cookies["refreshToken"];
            if (rawToken is not null)
                await _tokenService.RevokeAsync(rawToken);

            Response.Cookies.Delete("refreshToken");
            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var rawToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(rawToken))
                return Unauthorized();

            var result = await _tokenService.RefreshAsync(rawToken);
            if (result is null)
                return Unauthorized("Token expired or revoked");

            SetRefreshTokenCookie(result.Value.refreshToken);
            return Ok(new { token = result.Value.jwtToken });
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

        private void SetRefreshTokenCookie(string token)
        {
            var isSecure = !_environment.IsDevelopment();
            Response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,   // недоступен из JS
                Secure = isSecure, // только HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(30)
            });
        }
    }
}
