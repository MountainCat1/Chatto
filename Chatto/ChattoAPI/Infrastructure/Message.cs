using System.ComponentModel.DataAnnotations;

namespace Chatto.Infrastructure;

public class Message
{
    [Key]
    public Guid Guid { get; set; }
    public string Text { get; set; }
    public User Author { get; set; }
    public TextChannel TextChannel { get; set; }
    public DateTime Time { get; set; }
}