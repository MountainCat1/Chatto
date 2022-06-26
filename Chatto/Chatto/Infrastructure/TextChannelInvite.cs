using System.ComponentModel.DataAnnotations;

namespace Chatto.Infrastructure;

public class TextChannelInvite
{
    [Key]
    public Guid Guid { get; set; }
    public User Author { get; set; }
    public Guid AuthorGuid { get; set; }
    public User Target { get; set; }
    public Guid TargetGuid { get; set; }
    public TextChannel TextChannel { get; set; }
    public Guid TextChannelGuid { get; set; }
}