using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frederikskaj2.EmailAgent.Functions
{
    public class EmailAgent
    {
        private readonly ILogger<EmailAgent> logger;
        private readonly IStateProvider stateProvider;
        private readonly ImapOptions options;
        private readonly IEnumerable<IEmailProcessor> processors;

        public EmailAgent(ILogger<EmailAgent> logger, IOptions<ImapOptions> options, IEnumerable<IEmailProcessor> processors, IStateProvider stateProvider)
        {
            this.logger = logger;
            this.processors = processors;
            this.stateProvider = stateProvider;

            this.options = options.Value;
        }

        [FunctionName(nameof(EmailAgent))]
        public async Task Run([TimerTrigger("0 */5 * * * *", RunOnStartup = true)] TimerInfo timerInfo)
        {
            logger.LogInformation("Starting");
            using var client = new ImapClient();
            logger.LogDebug("Connecting to {HostName}:{Port}", options.HostName, options.Port);
            await client.ConnectAsync(options.HostName, options.Port);
            logger.LogDebug("Authenticating {UserName}", options.UserName);
            await client.AuthenticateAsync(options.UserName, options.Password);
            logger.LogInformation("Connected");
            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadOnly);

            var state = await stateProvider.Read();
            if (state.HighestModSeq >= inbox.HighestModSeq)
            {
                logger.LogInformation("Mailbox is unchanged");
                return;
            }

            var historyQuery = state.HighestModSeq == 0
                ? SearchQuery.DeliveredAfter(DateTime.UtcNow.Subtract(options.MaximumMessageAge))
                : SearchQuery.ChangedSince(state.HighestModSeq);
            foreach (var processor in processors)
            {
                logger.LogInformation("Processing {0}", processor.Description);
                var query = SearchQuery.And(historyQuery, processor.Query);
                var ids = await inbox.SearchAsync(query);
                foreach (var id in ids)
                {
                    var message = await inbox.GetMessageAsync(id);
                    if (state.KnownMessageIds.Contains(message.MessageId))
                    {
                        logger.LogDebug("Skipping known message id {MessageId}", message.MessageId);
                        continue;
                    }
                    var isProcessed = await processor.Process(message);
                    if (isProcessed)
                    {
                        logger.LogDebug("Adding message id {MessageId} to known message IDs", message.MessageId);
                        state.KnownMessageIds.Add(message.MessageId);
                    }
                }
            }

            state.HighestModSeq = inbox.HighestModSeq;
            client.Disconnect(true);

            await stateProvider.Write(state);
        }
    }
}
