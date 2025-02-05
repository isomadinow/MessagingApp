using System.Diagnostics;
using MessagingApp.DAL.Interfaces;
using MessagingApp.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Text.Json;

namespace MessagingApp.DAL.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IConfiguration configuration, ILogger<MessageRepository> logger)
        {
            var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";

            if (environment == "Production")
            {
                _connectionString = configuration["ConnectionStrings__DefaultConnection"]
                                    ?? configuration["ConnectionStrings:DefaultConnection"]
                                    ?? throw new InvalidOperationException("Строка подключения не задана в переменных окружения.");
            }
            else
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection")
                                    ?? throw new InvalidOperationException("Строка подключения не найдена в appsettings.json.");
            }

            _logger = logger;
        }

        public async Task SaveMessageAsync(Message message)
        {
            var sql = "INSERT INTO messages (text, timestamp, message_number) VALUES (@text, @timestamp, @messageNumber)";
            var stopwatch = Stopwatch.StartNew(); // Засекаем время выполнения

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("text", message.Text);
                cmd.Parameters.AddWithValue("timestamp", message.Timestamp);
                cmd.Parameters.AddWithValue("messageNumber", message.MessageNumber);
                await cmd.ExecuteNonQueryAsync();

                stopwatch.Stop();
                _logger.LogInformation("Сообщение сохранено: {MessageNumber}. Время выполнения: {ElapsedMilliseconds} мс", 
                                       message.MessageNumber, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка сохранения сообщения. Данные: {Message}", JsonSerializer.Serialize(message));
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime start, DateTime end)
        {
            var sql = "SELECT id, text, timestamp, message_number FROM messages WHERE timestamp BETWEEN @start AND @end ORDER BY timestamp";
            var messages = new List<Message>();
            var stopwatch = Stopwatch.StartNew(); // Засекаем время выполнения

            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("start", start);
                cmd.Parameters.AddWithValue("end", end);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    messages.Add(new Message
                    {
                        Id = reader.GetInt32(0),
                        Text = reader.GetString(1),
                        Timestamp = reader.GetDateTime(2),
                        MessageNumber = reader.GetInt32(3)
                    });
                }

                stopwatch.Stop();
                _logger.LogInformation("Получено {Count} сообщений. Время выполнения: {ElapsedMilliseconds} мс", 
                                       messages.Count, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка получения сообщений. Дата начала: {Start}, Дата конца: {End}", start, end);
                throw;
            }

            return messages;
        }
    }
}
