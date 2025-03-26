using PainKiller.CommandPrompt.CoreLib.Configuration.Contracts;
using PainKiller.CommandPrompt.CoreLib.Configuration.DomainObjects;
using PainKiller.CommandPrompt.CoreLib.Configuration.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PainKiller.CommandPrompt.CoreLib.Configuration.Services;

public class ConfigurationService : IConfigurationService
    {
        private ConfigurationService() { }

        private static readonly Lazy<IConfigurationService> Lazy = new(() => new ConfigurationService());
        public static IConfigurationService Service => Lazy.Value;
        public YamlContainer<T> Get<T>(string inputFileName = "") where T : new()
        {
            var fileName = string.IsNullOrEmpty(inputFileName) ? $"{typeof(T).Name}.yaml".GetSafePathRegardlessHowApplicationStarted() : inputFileName.GetSafePathRegardlessHowApplicationStarted();
            var yamlContent = File.ReadAllText(fileName);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            try
            {
                return deserializer.Deserialize<YamlContainer<T>>(yamlContent);
            }
            catch (Exception)
            {
                Console.WriteLine($"Could not deserialize the configuration file, default configuration will be loaded instead\nA template configuration file named default_{typeof(T).Name}.yaml will be created in application root.");
                var defaultConfig = new T();
                SaveChanges(defaultConfig, $"default_{typeof(T).Name}.yaml");
                return new YamlContainer<T>();
            }
        }
        public string SaveChanges<T>(T configuration, string inputFileName = "") where T : new()
        {
            if (configuration is null) return "";
            var fileName = string.IsNullOrEmpty(inputFileName) ? $"{configuration.GetType().Name}.yaml".GetSafePathRegardlessHowApplicationStarted() : inputFileName.GetSafePathRegardlessHowApplicationStarted();

            var yamlContainer = new YamlContainer<T> { Configuration = configuration, Version = "1.0" };
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yamlData = serializer.Serialize(yamlContainer);
            File.WriteAllText(fileName, yamlData);
            return fileName;
        }
    }