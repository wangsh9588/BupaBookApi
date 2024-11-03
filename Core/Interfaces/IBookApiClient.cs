using Core.Models;

namespace Core.Interfaces
{
    public interface IBookApiClient
    {
        Task<ApiResult<IEnumerable<BookOwner>>> GetBookOwnersAsync();
    }
}
