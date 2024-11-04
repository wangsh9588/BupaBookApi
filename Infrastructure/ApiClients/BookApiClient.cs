using Core.Interfaces;
using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Net;

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

            try
            {
                var response = await _httpClient.GetAsync("bookowners").ConfigureAwait(false);
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    };
                    apiResult = string.IsNullOrEmpty(responseContent)
                        ? ApiResult<IEnumerable<BookOwner>>.Failure(ApiError.Create(HttpStatusCode.NotFound, ErrorMessages.BookOwnersNotFound))
                        : ApiResult<IEnumerable<BookOwner>>.Success(JsonConvert.DeserializeObject<IEnumerable<BookOwner>>(responseContent, jsonSettings));
                }
                else
                {
                    _logger?.Error($"{loggerPrefix} Failed to get book owners. Error code: {response.StatusCode}, Error message: {responseContent}");
                    apiResult = ApiResult<IEnumerable<BookOwner>>.Failure(ApiError.Create(response.StatusCode, responseContent));
                }
            }
            catch (JsonException ex)
            {
                _logger?.Error($"{loggerPrefix} {ErrorMessages.DeserializeError} Error message: {ex.Message}");
                apiResult = ApiResult<IEnumerable<BookOwner>>.Failure(ApiError.Create(HttpStatusCode.InternalServerError, ErrorMessages.DeserializeError));
            }
            catch (Exception ex)
            {
                _logger?.Error($"{loggerPrefix} {ErrorMessages.UnexpectedError} Error message: {ex.Message}");
                apiResult = ApiResult<IEnumerable<BookOwner>>.Failure(ApiError.Create(HttpStatusCode.InternalServerError, ex.Message));
            }

            return apiResult;
        }
    }
}
