using System.Collections.Generic;

namespace Frederikskaj2.EmailAgent.Functions
{
    public class State
    {
        public ulong HighestModSeq { get; set; }
        public HashSet<string> KnownMessageIds { get; set; } = new HashSet<string>();
    }
}
