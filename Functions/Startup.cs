using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Frederikskaj2.EmailAgent.Functions.Startup))]

namespace Frederikskaj2.EmailAgent.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
            => builder.Services
                .AddHttpClient()
                .AddSingleton<IEmailProcessor, HomePageEmailProcessor>()
                .AddSingleton<IStateProvider, StateProvider>()
                .AddSingleton<IStorageProvider, StorageProvider>()
                .AddSingleton<ITeamsIntegration, TeamsIntegration>()
                .ConfigureOptions<ImapOptions>("Imap")
                .ConfigureOptions<StorageOptions>("Storage")
                .ConfigureOptions<TeamsOptions>("Teams");
    }
}
