using Core.Models;
using FluentAssertions;
using Infrastructure.ApiClients;
using Moq;
using Moq.Protected;
using Serilog;
using System.Net;
using System.Text.Json;

namespace BupaBookApi.Tests.UnitTests
{
    public class BookApiClientTests : IClassFixture<TestDataHelper>
    {
        private readonly TestDataHelper _testDataHelper;
        private readonly Mock<ILogger> _loggerMock = new();

        public BookApiClientTests(TestDataHelper testDataHelper)
        {
            _testDataHelper = testDataHelper;
        }

        [Fact]
        public async Task GetBookOwnersAsync_Should_ThrowJsonException_WhenResponseIsInvalid()
        {
            // Arrange
            var httpClient = CreateMockHttpClient("Invalid JSON", HttpStatusCode.OK);

            // Act
            var bookApiClient = new BookApiClient(httpClient, _loggerMock.Object);
            var result = await bookApiClient.GetBookOwnersAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Be(ErrorMessages.DeserializeError);
            result.Error.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GetBookOwnersAsync_Should_ReturnFailure_WhenResponseFailed()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(ErrorMessages.UnexpectedError, HttpStatusCode.InternalServerError);

            // Act
            var bookApiClient = new BookApiClient(httpClient, _loggerMock.Object);
            var result = await bookApiClient.GetBookOwnersAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Be(ErrorMessages.UnexpectedError);
            result.Error.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GetBookOwnersAsync_Should_ReturnNotFound_WhenResponseIsEmpty()
        {
            // Arrange
            var httpClient = CreateMockHttpClient("", HttpStatusCode.OK);

            // Act
            var bookApiClient = new BookApiClient(httpClient, _loggerMock.Object);
            var result = await bookApiClient.GetBookOwnersAsync();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Be(ErrorMessages.BookOwnersNotFound);
            result.Error.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBookOwnersAsync_Should_ReturnSuccess_WhenResponseIsValid()
        {
            // Arrange
            var bookOwners = _testDataHelper.GetBookOwners();
            var httpClient = CreateMockHttpClient(JsonSerializer.Serialize(bookOwners), HttpStatusCode.OK);

            // Act
            var bookApiClient = new BookApiClient(httpClient, _loggerMock.Object);
            var result = await bookApiClient.GetBookOwnersAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().BeAssignableTo(typeof(IEnumerable<BookOwner>));
        }

        private HttpClient CreateMockHttpClient(string responseContent, HttpStatusCode statusCode)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });
            return new HttpClient(httpMessageHandlerMock.Object) { BaseAddress = new Uri("https://digitalcodingtest.bupa.com.au/api/v1/") };
        }
    }
}
