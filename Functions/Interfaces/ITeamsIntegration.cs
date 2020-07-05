using System.Threading.Tasks;

namespace Frederikskaj2.EmailAgent.Functions
{
    public interface ITeamsIntegration
    {
        Task PostMessage(string channelName, string json);
    }
}
