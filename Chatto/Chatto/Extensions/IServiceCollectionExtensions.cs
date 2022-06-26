namespace Chatto.Extensions;

public static class ConfigurationManagerExtensions
{
    /// <summary>
    /// Loads files to configuration manager, binds section to type T and adds it as a singleton to service collection.
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="file"></param>
    /// <param name="section"></param>
    /// <param name="serviceCollection"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T AddConfigurationFile<T>(this ConfigurationManager configuration, 
        string file, string section, IServiceCollection serviceCollection)
    where T : class, new()
    {
        configuration.AddJsonFile(file);
        var configurationObject = new T();
        configuration.GetSection(section).Bind(configurationObject);
        serviceCollection.AddSingleton(configurationObject);
        return configurationObject;
    }
}