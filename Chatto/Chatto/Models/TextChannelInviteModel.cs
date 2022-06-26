namespace Chatto.Models;

public class TextChannelInviteModel
{
    public Guid TargetUserGuid { get; set; }
    public Guid TextChannelGuid { get; set; }
}