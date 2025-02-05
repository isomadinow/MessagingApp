using System.Net.WebSockets;

namespace MessagingApp.Services;

public interface IWebSocketService
{
    Task HandleWebSocketAsync(WebSocket ws, CancellationToken cancellationToken);
    Task BroadcastMessageAsync(string message, CancellationToken cancellationToken);
}