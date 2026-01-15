using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;

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
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // cookie отправляется по тому же протоколу, что и запрос (HTTP → HTTP, HTTPS → HTTPS)
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Время жизни cookie для OAuth flow
            })
        .AddGoogle(options => 
            {
                var cliendId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
                if (cliendId == null || clientSecret == null)
                {
                    throw new ArgumentNullException("Не указаны переменные окружения GOOGLE_CLIENT_ID или GOOGLE_CLIENT_SECRET");
                }
                options.ClientId = cliendId;
                options.ClientSecret = clientSecret;
                options.SaveTokens = true; // Сохраняем токены Google для дальнейшего использования, если нужно
                options.SignInScheme = "ExternalCookies"; // Используем cookie-схему для временного хранения данных OAuth
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

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Notes Client");
app.UseRouting();       // маршрутизация
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();   // регистрирует контроллеры

app.Run();
