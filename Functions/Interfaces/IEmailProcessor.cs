using System.Threading.Tasks;
using MailKit.Search;
using MimeKit;

namespace Frederikskaj2.EmailAgent.Functions
{
    public interface IEmailProcessor
    {
        string Description { get; }
        SearchQuery Query { get; }
        Task<bool> Process(MimeMessage message);
    }
}
