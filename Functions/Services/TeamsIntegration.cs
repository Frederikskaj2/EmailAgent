using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frederikskaj2.EmailAgent.Functions
{
    public class TeamsIntegration : ITeamsIntegration
    {
        private readonly ILogger logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly TeamsOptions options;

        public TeamsIntegration(ILogger<TeamsIntegration> logger, IOptions<TeamsOptions> options, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;

            this.options = options.Value;
        }

        public async Task PostMessage(string channelName, string json)
        {
            if (!options.Webhooks.TryGetValue(channelName, out var webhookUrl))
                throw new ArgumentException($"Unknown channel name {channelName}.", nameof(channelName));

            if (!options.IsPostingEnabled)
            {
                logger.LogInformation("Posting to Teams channel {ChannelName} is disabled", channelName);
                return;
            }

            logger.LogInformation("Posting to Teams channel {ChannelName}", channelName);
            var httpClient = httpClientFactory.CreateClient();
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(webhookUrl, content);
        }
    }
}
