using System.ComponentModel.DataAnnotations;

namespace Chatto.Infrastructure;

public class TextChannel
{
    [Key]
    public Guid Guid { get; set; }
    public TextChannelType ChannelType { get; set; }
    public virtual List<User> Users { get; set; }
    public virtual List<Message> Messages { get; set; }
}

public enum TextChannelType
{
    DirectMessage,
    Group
}

