using MessagingApp.Models.DTOs;
using MessagingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageService messageService, ILogger<MessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Отправка сообщения.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageCreateDto dto)
        {
            var traceId = HttpContext.TraceIdentifier;
            _logger.LogInformation("Запрос на создание сообщения {TraceId}: {MessageText}", traceId, dto.Text);

            try
            {
                await _messageService.SaveMessageAsync(dto);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Ошибка валидации {TraceId}: {Message}", traceId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке запроса {TraceId}", traceId);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Получение списка сообщений за указанный период.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var traceId = HttpContext.TraceIdentifier;
            _logger.LogInformation("Запрос на получение сообщений {TraceId}: Start={Start}, End={End}", traceId, start, end);

            try
            {
                var messages = await _messageService.GetMessagesAsync(start, end);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке запроса {TraceId}", traceId);
                return StatusCode(500);
            }
        }
    }
}
