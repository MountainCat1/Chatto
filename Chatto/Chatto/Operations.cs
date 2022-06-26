namespace Chatto;

public static class Operations
{
    // Resource specific policies
    public static string SendMessage { get; } = "SendMessagePolicy";
    public static string View { get; } = "ViewPolicy";
    public static string InviteNewMembers { get; } = "InviteNewMembers";
}