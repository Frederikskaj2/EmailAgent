using System.Threading.Tasks;

namespace Frederikskaj2.EmailAgent.Functions
{
    public interface IStorageProvider
    {
        Task<T?> Read<T>(string name) where T : class;
        Task Write<T>(string name, T value) where T : class;
    }
}
