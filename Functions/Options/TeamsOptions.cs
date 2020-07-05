using System;
using System.Collections.Generic;

namespace Frederikskaj2.EmailAgent.Functions
{
    public class TeamsOptions
    {
        public bool IsPostingEnabled { get; set; }
        public Dictionary<string, Uri> Webhooks { get; set; } = new Dictionary<string, Uri>();
    }
}
