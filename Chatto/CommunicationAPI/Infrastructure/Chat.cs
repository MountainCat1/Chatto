using System.ComponentModel.DataAnnotations;

namespace CommunicationAPI.Infrastructure;

public class TextChannel
{
    [Key]
    public Guid Guid { get; set; }
    public TextChannelType ChannelType { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}

public enum TextChannelType
{
    DirectMessage,
    Group
}

