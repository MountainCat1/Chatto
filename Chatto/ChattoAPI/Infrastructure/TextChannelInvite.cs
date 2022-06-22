using System.ComponentModel.DataAnnotations;

namespace Chatto.Infrastructure;

public class TextChannelInvite
{
    [Key]
    public Guid Guid { get; set; }
    public User Author { get; set; }
    public User Target { get; set; }
    public TextChannel TextChannel { get; set; }
}