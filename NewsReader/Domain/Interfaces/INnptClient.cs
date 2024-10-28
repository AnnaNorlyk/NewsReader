using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsReader
{
    public interface INntpClient
    {
        Task ConnectAsync(string hostname, int port);
        Task AuthenticateAsync(string username, string password);
        Task<List<string>> GetNewsgroupsAsync();
        Task<string> GetArticleAsync(string articleNumber);
        Task<List<(int ArticleNumber, string Subject)>> GetArticleHeadersAsync(string newsgroupName);
        void Disconnect();
    }
}