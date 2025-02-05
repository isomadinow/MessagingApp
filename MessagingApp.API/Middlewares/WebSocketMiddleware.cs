using MessagingApp.Services;


namespace MessagingApp.Middlewares;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebSocketService _webSocketService;
    private readonly ILogger<WebSocketMiddleware> _logger;

    public WebSocketMiddleware(RequestDelegate next, IWebSocketService webSocketService, ILogger<WebSocketMiddleware> logger)
    {
        _next = next;
        _webSocketService = webSocketService;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var ws = await context.WebSockets.AcceptWebSocketAsync();
                await _webSocketService.HandleWebSocketAsync(ws, context.RequestAborted);
            }
            else
            {
                _logger.LogWarning("Попытка подключиться к WebSocket без WebSocket-запроса");
                context.Response.StatusCode = 400;
            }
        }
        else
        {
            await _next(context);
        }
    }
}