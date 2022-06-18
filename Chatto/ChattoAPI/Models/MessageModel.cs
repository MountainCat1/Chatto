namespace Chatto.Models;

public class MessageModel
{
    public Guid Guid { get; set; }
    public string Text { get; set; }
    public Guid AuthorGuid { get; set; }
    public Guid TextChannelGuid { get; set; }
    public DateTime Time { get; set; }
}