using System;

namespace Frederikskaj2.EmailAgent.Functions
{
    public class ImapOptions
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; } = 993;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public TimeSpan MaximumMessageAge { get; set; } = TimeSpan.FromDays(7);
    }
}
