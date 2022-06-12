namespace Chatto.Configuration;

public class MicroservicesSettings
{
    public Authentication AuthenticationSettings { get; set; }

    public class Authentication
    {
        public string Url { get; set; }
    }
}