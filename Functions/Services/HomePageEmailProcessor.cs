using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Frederikskaj2.EmailAgent.Functions.Extensions;
using Frederikskaj2.EmailAgent.Functions.Properties;
using MailKit.Search;
using MimeKit;

namespace Frederikskaj2.EmailAgent.Functions
{
    internal class HomePageEmailProcessor : IEmailProcessor
    {
        private static readonly Regex bodyRegex = new Regex(@"^Fra: (?<from>.+) <(?<email>[^>]+)>\r?\nEmne: (?<subject>.*?)\r?\n\r?\n(?<text>.*)\r?\n--\s*\r?\n", RegexOptions.Singleline);
        private static readonly Regex subjectRegex = new Regex(@"^(?<type>.*) - "".*""$", RegexOptions.Singleline);

        private readonly ITeamsIntegration teamsIntegration;

        public HomePageEmailProcessor(ITeamsIntegration teamsIntegration) => this.teamsIntegration = teamsIntegration;

        public string Description => "home page mail";

        public SearchQuery Query => SearchQuery.HeaderContains("Sender", "wordpress=frederikskaj2.dk@mail1.ejerforeningenfrederikskaj2.dk");

        public async Task<bool> Process(MimeMessage message)
        {
            var parsedMessage = ParseMessage(message);
            if (parsedMessage == null)
                return false;
            var json = GetMessageCard(parsedMessage);
            await teamsIntegration.PostMessage("Beboerhenvendelser", json);
            return true;
        }

        private Message? ParseMessage(MimeMessage message)
        {
            var subjectMatch = subjectRegex.Match(message.Subject);
            if (!subjectMatch.Success)
                return null;
            var type = subjectMatch.Groups["type"].Value;
            var bodyMatch = bodyRegex.Match(message.TextBody);
            if (!bodyMatch.Success)
                return null;
            var from = bodyMatch.Groups["from"].Value;
            var email = bodyMatch.Groups["email"].Value;
            var subject = bodyMatch.Groups["subject"].Value;
            var text = bodyMatch.Groups["text"].Value;
            return new Message(from, email, type, subject, text, message.Subject);
        }

        private static string GetMessageCard(Message message)
        {
            var audience = GetAudience();

            var title = GetTitle().EscapeJsonString();
            return Encoding.UTF8.GetString(Resources.HomePageEmail)
                .Replace("{TITLE}", title)
                .Replace("{SUMMARY-START}", GetSummaryStart())
                .Replace("{SUBJECT}", message.Subject.EscapeMarkdown().EscapeJsonString())
                .Replace("{FROM}", message.From.EscapeMarkdown().EscapeJsonString())
                .Replace("{EMAIL}", message.Email.EscapeMarkdown().EscapeJsonString())
                .Replace("{TEXT}", message.Text.EscapeMarkdown().EscapeJsonString())
                .Replace("{SEARCH}", Uri.EscapeDataString(message.EmailSubject));

            Audience GetAudience()
            {
                if (message.Type.StartsWith("Bestyrelses-henvendelse", StringComparison.Ordinal))
                    return Audience.Board;
                if (message.Type.StartsWith("Vicevært-henvendelse", StringComparison.Ordinal))
                    return Audience.Janitor;
                if (message.Type.StartsWith("Administrator-henvendelse", StringComparison.Ordinal))
                    return Audience.Administrator;
                if (message.Type.StartsWith("Bådpladser", StringComparison.Ordinal))
                    return Audience.Marina;
                return Audience.Unknown;
            }

            string GetTitle() => audience switch
            {
                Audience.Board => "Beboerhenvendelse til bestyrelsen",
                Audience.Janitor => "Beboerhenvendelse til viceværten",
                Audience.Administrator => "Beboerhenvendelse til administrator",
                Audience.Marina => "Henvendelse om bådpladser",
                _ => "Henvendelse fra hjemmeside"
            };

            string GetSummaryStart() => audience switch
            {
                Audience.Board => "Til bestyrelsen",
                Audience.Janitor => "Til viceværten",
                Audience.Administrator => "Til administrator",
                Audience.Marina => "Bådpladser",
                _ => "Henvendelse"
            };
        }

        private class Message
        {
            public Message(string from, string email, string type, string subject, string text, string emailSubject)
            {
                From = from;
                Email = email;
                Type = type;
                Subject = subject;
                Text = text;
                EmailSubject = emailSubject;
            }

            public string From { get; }
            public string Email { get; }
            public string Type { get; }
            public string Subject { get; }
            public string Text { get; }
            public string EmailSubject { get; }
        }

        private enum Audience
        {
            Unknown,
            Board,
            Janitor,
            Administrator,
            Marina
        }
    }
}
