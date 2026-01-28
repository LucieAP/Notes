using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using server.Interfaces;

// Загружаем .env файл
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Получаем шаблон строки подключения
var connTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
// Расширяем плейсхолдеры напрямую
var connectionString = Environment.ExpandEnvironmentVariables(connTemplate);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(option => 
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => 
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,      // Issue - кто выпустил токен
                    ValidateAudience = true,    // Audience - кому выпущен токен
                    ValidateLifetime = true,    // Проверка срока действия токена
                    ValidateIssuerSigningKey = true,    // Проверка ключа подписи токена
                    ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),  
                    ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT")))
                };
                
            })
        .AddCookie("ExternalCookies", options =>
            {
                // Настройки для временного хранения данных OAuth между редиректами
                options.Cookie.Name = "ExternalCookies";
                options.Cookie.HttpOnly = true; // Предотвращает доступ к cookie через JavaScript в браузере
                options.Cookie.SameSite = SameSiteMode.Lax;  // ограничение cross-site запросов
                options.Cookie.Path = "/"; // Устанавливаем path в корень для доступности на всех путях
                
                if (builder.Environment.IsDevelopment())
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                }
                else
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }
                
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Время жизни cookie для OAuth flow
                options.SlidingExpiration = true; // Обновление времени истечения при активности
            })
        .AddGoogle(options => 
            {
                var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
                if (clientId == null || clientSecret == null)
                {
                    throw new ArgumentNullException("Не указаны переменные окружения GOOGLE_CLIENT_ID или GOOGLE_CLIENT_SECRET");
                }
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SaveTokens = true; // Сохраняем токены Google для дальнейшего использования, если необходимо
                options.SignInScheme = "ExternalCookies"; // Используем cookie-схему для временного хранения данных OAuth
                
                // Явно указываем scopes для получения всех необходимых данных
                options.Scope.Add("openid");
                options.Scope.Add("profile"); // Для picture, name, given_name, family_name
                options.Scope.Add("email");   // Для email и email_verified
                
                // Добавляем дополнительные Claims из JSON ответа Google
                options.Events.OnCreatingTicket = context =>
                {
                    // Получаем данные из JSON ответа Google (context.User - это JsonElement)
                    if (context.User.TryGetProperty("picture", out var pictureElement))
                    {
                        var picture = pictureElement.GetString();
                        if (!string.IsNullOrEmpty(picture))
                        {
                            context.Identity?.AddClaim(new System.Security.Claims.Claim("picture", picture));
                        }
                    }
                    
                    if (context.User.TryGetProperty("email_verified", out var emailVerifiedElement))
                    {
                        var emailVerified = emailVerifiedElement.GetBoolean().ToString();
                        context.Identity?.AddClaim(new System.Security.Claims.Claim("email_verified", emailVerified));
                    }
                    
                    return System.Threading.Tasks.Task.CompletedTask;
                };
                
                // Настройка correlation cookie для правильной работы OAuth state
                options.CorrelationCookie.Name = ".AspNetCore.Correlation.Google";
                options.CorrelationCookie.HttpOnly = true;
                options.CorrelationCookie.Path = "/";
                
                // Так как приложение работает по HTTPS, используем Always для SecurePolicy
                // SameSiteMode.Lax работает для OAuth редиректов в том же домене
                if (builder.Environment.IsDevelopment())
                {
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    // Используем Always, так как работаем по HTTPS
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                }
                else
                {
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                }
            });

builder.Services.AddCors(options => 
    {
        options.AddPolicy("Notes Client", policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
        });
    });

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();       // маршрутизация
app.UseCors("Notes Client");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();   // регистрирует контроллеры

app.Run();
