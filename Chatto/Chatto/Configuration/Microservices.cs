namespace Chatto.Configuration;

public class MicroservicesSettings
{
    public Authentication AuthenticationSettings { get; set; }
    public Guid GuidSettings { get; set; }

    public class Authentication
    {
        public string Url { get; set; }
    }
    public class Guid
    {
        public string Url { get; set; }
    }
}