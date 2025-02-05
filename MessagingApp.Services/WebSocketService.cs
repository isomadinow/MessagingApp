using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MessagingApp.Services;

public class WebSocketService : IWebSocketService
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();
    private readonly ILogger<WebSocketService> _logger;

    public WebSocketService(ILogger<WebSocketService> logger)
    {
        _logger = logger;
    }

    public async Task HandleWebSocketAsync(WebSocket ws, CancellationToken cancellationToken)
    {
        var clientId = Guid.NewGuid();
        _clients.TryAdd(clientId, ws);
        _logger.LogInformation("WebSocket-клиент подключен {ClientId}, Всего подключений: {Count}", clientId, _clients.Count);

        var buffer = new byte[1024 * 4];

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation("WebSocket-клиент {ClientId} отключается", clientId);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Закрытие", cancellationToken);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation("WebSocket получил сообщение {ClientId}: {Message}", clientId, message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка в WebSocket-соединении {ClientId}", clientId);
        }
        finally
        {
            RemoveWebSocket(clientId);
            _logger.LogInformation("WebSocket-клиент {ClientId} отключился, осталось: {Count}", clientId, _clients.Count);
        }
    }

    public async Task BroadcastMessageAsync(string message, CancellationToken cancellationToken)
    {
        var messageBuffer = Encoding.UTF8.GetBytes(message);

        foreach (var (clientId, ws) in _clients)
        {
            if (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, cancellationToken);
                _logger.LogInformation("Сообщение отправлено WebSocket-клиенту {ClientId}", clientId);
            }
        }
    }

    private void RemoveWebSocket(Guid clientId)
    {
        _clients.TryRemove(clientId, out _);
    }
}
