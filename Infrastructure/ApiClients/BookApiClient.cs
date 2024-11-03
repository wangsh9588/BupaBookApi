using Core.Interfaces;
using Core.Models;
using Serilog;
using System.Net;
using System.Text.Json;

namespace Infrastructure.ApiClients
{
    public class BookApiClient : IBookApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public BookApiClient(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger.ForContext<BookApiClient>();
        }

        public async Task<ApiResult<IEnumerable<BookOwner>>> GetBookOwnersAsync()
        {
            var loggerPrefix = $"{nameof(BookApiClient)}:{nameof(GetBookOwnersAsync)} -";
            ApiResult<IEnumerable<BookOwner>> apiResult;

            var response = await _httpClient.GetAsync("bookowners").ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
            {
                apiResult = ApiResult<IEnumerable<BookOwner>>.Success(JsonSerializer.Deserialize<IEnumerable<BookOwner>>(responseContent));
            }
            else
            {
                _logger?.Error($"{loggerPrefix} Failed to get book owners. Error code: {response.StatusCode}, Error message: {responseContent}");
                apiResult = ApiResult<IEnumerable<BookOwner>>
                    .Failure(ApiError.Create(
                        string.IsNullOrEmpty(responseContent) ? HttpStatusCode.NotFound : response.StatusCode,
                        string.IsNullOrEmpty(responseContent) ? ErrorMessages.BooksNotFound : responseContent));
            }

            return apiResult;
        }
    }
}
