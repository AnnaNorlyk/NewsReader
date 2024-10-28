using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsReader
{
    public interface INewsgroupService
    {
        Task<IEnumerable<Newsgroup>> GetAllNewsgroupsAsync();
    }
}
