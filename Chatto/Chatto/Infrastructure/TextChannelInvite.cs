using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatto.Infrastructure;

public class TextChannelInvite
{
    [Key]
    public Guid Guid { get; set; }
    public User Author { get; set; }
    [ForeignKey(nameof(Author))] public Guid AuthorGuid { get; set; }
    public User Target { get; set; }
    [ForeignKey(nameof(Target))] public Guid TargetGuid { get; set; }
    //public int TargetAccountId { get; set; }
    public TextChannel TextChannel { get; set; }
    [ForeignKey(nameof(TextChannel))] public Guid TextChannelGuid { get; set; }
}