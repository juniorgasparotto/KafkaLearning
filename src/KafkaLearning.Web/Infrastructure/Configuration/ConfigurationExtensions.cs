using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KafkaLearning.Web.Infrastructure.Configurations;

namespace KafkaLearning.Web.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<AppConfigurationOptions>(configuration);
            return serviceCollection;
        }

    }
}