using System.Threading.Tasks;

namespace Frederikskaj2.EmailAgent.Functions
{
    public interface IStateProvider
    {
        Task<State> Read();
        Task Write(State state);
    }
}
