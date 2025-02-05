using MessagingApp.Models.DTOs;
using MessagingApp.Models.Entities;

namespace MessagingApp.Services;

public interface IMessageService
{
    Task SaveMessageAsync(MessageCreateDto dto);
    Task<IEnumerable<Message>> GetMessagesAsync(DateTime start, DateTime end);
}