namespace Shared.Models;

public class RegisterGoogleModel
{
    public string GoogleId { get; set; }
    public string FullName { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public Uri ImageUrl { get; set; }
    public string Email { get; set; }
}