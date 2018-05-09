using System.IO;
using Microsoft.Extensions.Configuration;

namespace RabbitMQSeminar
{
    public class Settings
    {
        private static readonly IConfigurationRoot _settings;

        static Settings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _settings = builder.Build();
        }

        public static string GetSetting(string name)
        {
            return _settings[name];
        }
    }
}