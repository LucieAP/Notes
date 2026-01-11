using Microsoft.EntityFrameworkCore;
using DotNetEnv;

// Загружаем .env файл
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Получаем шаблон строки подключения
var connTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
// Расширяем плейсхолдеры напрямую
var connectionString = Environment.ExpandEnvironmentVariables(connTemplate);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();       // маршрутизация
app.MapControllers();   // регистрирует контроллеры

app.Run();
