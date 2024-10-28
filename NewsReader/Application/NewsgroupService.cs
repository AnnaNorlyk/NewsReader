using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsReader
{
    public class NewsgroupService : INewsgroupService
    {
        private readonly INntpClient _nntpClient;

        public NewsgroupService(INntpClient nntpClient)
        {
            _nntpClient = nntpClient;
        }

        public async Task<IEnumerable<Newsgroup>> GetAllNewsgroupsAsync()
        {
            var newsgroupNames = await _nntpClient.GetNewsgroupsAsync();
            return newsgroupNames.Select(name => new Newsgroup { Name = name });
        }

    }
}

