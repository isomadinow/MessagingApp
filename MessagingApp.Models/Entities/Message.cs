namespace MessagingApp.Models.Entities;

public class Message
{
    public int Id { get; set; } // Primary Key
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }
    public int MessageNumber { get; set; }
}