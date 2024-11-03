using Core.Models;
using Microsoft.Extensions.Configuration;

namespace BupaBookApi.Tests
{
    public class TestConfigurationBuilder
    {
        private IConfigurationRoot _configurationRoot;
        public TestConfigurationBuilder Build()
        {
            _configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false)
                .Build();

            return this;
        }

        public BookOwnerConfig GetBookOwnerConfig()
        {
            return GetConfigInternal<BookOwnerConfig>();
        }

        private T GetConfigInternal<T>() where T : new()
        {
            var config = new T();
            if (_configurationRoot == null)
            {
                this.Build();
            }

            _configurationRoot.GetSection(typeof(T).Name).Bind(config);
            return config;
        }
    }
}
