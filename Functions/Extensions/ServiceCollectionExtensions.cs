using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frederikskaj2.EmailAgent.Functions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureOptions<TOptions>(this IServiceCollection services, string sectionName) where TOptions : class
        {
            services
                .AddOptions<TOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration.GetSection(sectionName).Bind(options));
            return services;
        }
    }
}
