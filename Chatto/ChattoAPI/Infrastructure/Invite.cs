using System.ComponentModel.DataAnnotations;

namespace Chatto.Infrastructure;

public class TextChannelInvite
{
    [Key]
    public Guid Guid { get; set; }
    public User Sender { get; set; }
    public User Receiver { get; set; }
    public TextChannel TextChannel { get; set; }
}