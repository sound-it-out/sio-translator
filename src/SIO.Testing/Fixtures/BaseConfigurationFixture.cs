using System;
using Microsoft.Extensions.Configuration;

namespace SIO.Testing.Fixtures
{
    public class BaseConfigurationFixture
    {
        public IConfiguration Configuration { get; }

        public BaseConfigurationFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(GetType().Assembly, optional: true)
                .AddEnvironmentVariables(prefix: "SIO_")
                .Build();

            Configuration = configuration;
        }
    }
}
