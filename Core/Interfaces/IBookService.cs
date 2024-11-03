using Core.Models;

namespace Core.Interfaces
{
    public interface IBookService
    {
        Task<ServiceResult<CategorisedBooks>> GetBooksAsync(BookFilter bookFilter);
    }
}
