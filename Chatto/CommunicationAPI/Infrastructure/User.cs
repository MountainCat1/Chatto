using System.ComponentModel.DataAnnotations;

namespace CommunicationAPI.Infrastructure;

public class User
{
    [Key]
    public Guid Guid { get; set; }
    public string Username { get; set; }
    public int AccountId { get; set; }
    public virtual ICollection<TextChannel> Chats { get; set; }
}