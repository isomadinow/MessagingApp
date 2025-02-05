using MessagingApp.Models.Entities;

namespace MessagingApp.DAL.Interfaces;

public interface IMessageRepository
{
    Task SaveMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesAsync(DateTime start, DateTime end);
}