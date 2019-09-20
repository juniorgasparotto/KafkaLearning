using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using KafkaLearning.ServiceBus;
using KafkaLearning.ServiceBus.Extensions;
using KafkaLearning.ServiceBus.Logs;
using KafkaLearning.Web.Core.Entities;
using KafkaLearning.Web.Infrastructure.Configurations;

namespace KafkaLearning.Web.Infrastructure.Configuration
{
    public static class ServiceBusExtensions
    {
        /// <summary>
        /// Register interfaces and implementations on dependency injection framework.
        /// </summary>
        /// <param name="serviceCollection">Instance of ServiceCollection type.</param>
        /// <returns>The given serviceCollection instance with interfaces and implementations registered.</returns>
        public static IServiceCollection AddServiceBus(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddKafka();
            return serviceCollection;
        }
    }
}