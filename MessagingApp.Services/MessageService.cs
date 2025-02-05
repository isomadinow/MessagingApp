using System.Text.Json;
using MessagingApp.DAL.Interfaces;
using MessagingApp.Models.DTOs;
using MessagingApp.Models.Entities;
using Microsoft.Extensions.Logging;

namespace MessagingApp.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;
    private readonly ILogger<MessageService> _logger;
    private readonly IWebSocketService _webSocketService; 

    public MessageService(IMessageRepository repository, ILogger<MessageService> logger, IWebSocketService webSocketService)
    {
        _repository = repository;
        _logger = logger;
        _webSocketService = webSocketService;
    }

    public async Task SaveMessageAsync(MessageCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Text) || dto.Text.Length > 128)
        {
            throw new ArgumentException("Текст должен быть от 1 до 128 символов");
        }

        var message = new Message
        {
            Text = dto.Text,
            MessageNumber = dto.MessageNumber,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await _repository.SaveMessageAsync(message);
            _logger.LogInformation("Сообщение сохранено: {MessageNumber}", message.MessageNumber);

            // Отправляем WebSocket-клиентам
            var jsonMessage = JsonSerializer.Serialize(new
            {
                message.MessageNumber,
                message.Timestamp,
                message.Text
            });

            await _webSocketService.BroadcastMessageAsync(jsonMessage, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении сообщения");
            throw;
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime start, DateTime end)
    {
        try
        {
            var messages = await _repository.GetMessagesAsync(start, end);
            _logger.LogInformation("Получено {Count} сообщений", messages.Count());
            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении сообщений");
            throw;
        }
    }
}
