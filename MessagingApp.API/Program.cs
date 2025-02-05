using MessagingApp.DAL.Interfaces;
using MessagingApp.DAL.Repositories;
using MessagingApp.Services;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Загружаем переменные окружения
builder.Configuration.AddEnvironmentVariables();

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Регистрируем зависимости
builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>(); // Используем интерфейс WebSocketService
builder.Services.AddControllers();

// Настройка WebSocket
builder.Services.AddWebSockets(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Messaging API",
        Version = "v1",
        Description = "API для отправки и получения сообщений"
    });
});

// Настройки Kestrel
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.ConfigureKestrel((context, options) =>
    {
        options.Configure(context.Configuration.GetSection("Kestrel"));
        options.AddServerHeader = false;
    });
}

var app = builder.Build();

// Добавляем health-check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

// Настройка middleware
app.UseWebSockets();  // Включаем WebSockets
app.UseMiddleware<WebSocketMiddleware>();  // Обрабатываем WebSocket-соединения
app.UseRouting(); // Маршруты после WebSocketMiddleware

// Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Messaging API V1");
});

// Добавляем маршруты
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Запуск приложения
app.Run();