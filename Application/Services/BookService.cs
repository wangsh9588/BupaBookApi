using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;

namespace Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookApiClient _bookApiClient;
        private readonly BookOwnerConfig _bookOwnerConfig;
        private readonly ILogger _logger;
        private const string Adults = "Adults";
        private const string Children = "Children";

        public BookService(IBookApiClient bookApiClient, IOptions<BookOwnerConfig> bookOwnerConfig, ILogger logger)
        {
            _bookApiClient = bookApiClient ?? throw new ArgumentNullException(nameof(bookApiClient));
            _bookOwnerConfig = bookOwnerConfig?.Value ?? throw new ArgumentNullException(nameof(bookOwnerConfig));
            _logger = logger.ForContext<BookService>();
        }

        public async Task<ServiceResult<CategorisedBooks>> GetBooksAsync(BookFilter bookFilter)
        {
            var loggerPrefix = $"{nameof(BookService)}:{nameof(GetBooksAsync)} -";
            CategorisedBooks categorisedBooks = new();
            ServiceResult<CategorisedBooks> serviceResult;

            try
            {
                var bookOwnersApiResult = await _bookApiClient.GetBookOwnersAsync().ConfigureAwait(false);

                if (bookOwnersApiResult.IsSuccess)
                {
                    var groupedBookOwners = bookOwnersApiResult?.Data?.GroupBy(owner => owner.Age >= _bookOwnerConfig.MinimumAdultAge ? Adults : Children);
                    var adultBooks = GetBooksByAgeCategory(Adults, bookFilter, groupedBookOwners);
                    if (adultBooks?.Any() ?? false)
                        categorisedBooks.SetAdultBooks(adultBooks);

                    var childrenBooks = GetBooksByAgeCategory(Children, bookFilter, groupedBookOwners);
                    if (childrenBooks?.Any() ?? false)
                        categorisedBooks.SetChildrenBooks(childrenBooks);

                    if (categorisedBooks.AdultBooks.Any() || categorisedBooks.ChildrenBooks.Any())
                        serviceResult = ServiceResult<CategorisedBooks>.Success(categorisedBooks);
                    else
                        serviceResult = ServiceResult<CategorisedBooks>.Failure(HttpStatusCode.NotFound, ErrorMessages.BooksNotFound);
                }
                else
                {
                    serviceResult = ServiceResult<CategorisedBooks>.Failure(HttpStatusCodeMapper(bookOwnersApiResult.Error.StatusCode), bookOwnersApiResult.Error.Message);
                }
            }
            catch (Exception ex) 
            {
                _logger?.Error($"{loggerPrefix} {ErrorMessages.UnexpectedError} Error message: {ex.Message}");
                serviceResult = ServiceResult<CategorisedBooks>.Failure(HttpStatusCode.ServiceUnavailable, ex.Message);
            }
            return serviceResult;
        }

        private HashSet<string>? GetBooksByAgeCategory(string ageCategory, BookFilter bookFilter, IEnumerable<IGrouping<string, BookOwner>>? groupedBookOwners)
        {
            var categorisedBooks = groupedBookOwners
                ?.FirstOrDefault(g => g.Key.Equals(ageCategory))
                ?.SelectMany(category => category.Books);

            if ((int)bookFilter.BookType > (int)BookTypeEnum.All)
                categorisedBooks = categorisedBooks?.Where(book => book.Type.Equals(Enum.GetName(typeof(BookTypeEnum), bookFilter.BookType)));

            // Assumption: The term 'a list of all the books' means a list of book names
            return categorisedBooks
                ?.Select(book => book.Name)
                ?.OrderBy(bookName => bookName)
                ?.ToHashSet();
        }

        // Map downstream API status codes to the current API's standards 
        private HttpStatusCode HttpStatusCodeMapper(HttpStatusCode httpStatus)
        {
            HttpStatusCode resultHttpStatus;

            if ((int)httpStatus >= 200 && (int)httpStatus < 300)
            {
                resultHttpStatus = HttpStatusCode.OK;
            }
            else if ((int)httpStatus >= 300 && (int)httpStatus < 500)
            {
                resultHttpStatus = httpStatus;
            }
            else
            {
                resultHttpStatus = HttpStatusCode.ServiceUnavailable;
            }

            return resultHttpStatus;
        }
    }
}
