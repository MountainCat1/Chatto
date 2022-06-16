using System.ComponentModel.DataAnnotations;

namespace Chatto.Infrastructure;

public class User
{
    [Key]
    public Guid Guid { get; set; }
    public string Username { get; set; }
    public int AccountId { get; set; }
    public virtual ICollection<TextChannel> TextChannels { get; set; }
}