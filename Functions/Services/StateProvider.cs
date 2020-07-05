using System.Threading.Tasks;

namespace Frederikskaj2.EmailAgent.Functions
{
    internal class StateProvider : IStateProvider
    {
        private const string BlobName = "state.json";
        private readonly IStorageProvider storageProvider;

        public StateProvider(IStorageProvider storageProvider) => this.storageProvider = storageProvider;

        public async Task<State> Read() => await storageProvider.Read<State>(BlobName) ?? new State();

        public Task Write(State state) => storageProvider.Write(BlobName, state);
    }
}
