using Application.Services;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using System.Net;

namespace BupaBookApi.Tests.UnitTests
{
    public class BookServiceTests : IClassFixture<TestConfigurationBuilder>, IClassFixture<TestDataHelper>
    {
        private readonly TestConfigurationBuilder _testConfigurationBuilder;
        private readonly TestDataHelper _testDataHelper;
        private readonly Mock<IBookApiClient> _bookApiClientMock = new();
        private readonly Mock<ILogger> _loggerMock = new();

        public BookServiceTests(TestConfigurationBuilder testConfigurationBuilder, TestDataHelper testDataHelper)
        {
            _testConfigurationBuilder = testConfigurationBuilder.Build();
            _testDataHelper = testDataHelper;
        }

        [Fact]
        public async Task GetBooksAsync_Should_ReturnFailure_WhenApiClientFailed()
        {
            // Arrange
            var bookFilter = new BookFilter(BookTypeEnum.All);
            var errorMessage = "Too Many Requests";
            _bookApiClientMock.Setup(x => x.GetBookOwnersAsync())
                .ReturnsAsync(ApiResult<IEnumerable<BookOwner>>.Failure(ApiError.Create(HttpStatusCode.TooManyRequests, errorMessage)));

            // Act
            var bookService = new BookService(_bookApiClientMock.Object, Options.Create(_testConfigurationBuilder.GetBookOwnerConfig()), _loggerMock.Object);
            var result = await bookService.GetBooksAsync(bookFilter);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(errorMessage);
            result.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task GetBooksAsync_Should_ReturnFailure_WhenApiClientExceptionOccurred()
        {
            // Arrange
            var bookFilter = new BookFilter(BookTypeEnum.All);
            var errorMessage = "An error occurred";
            _bookApiClientMock.Setup(x => x.GetBookOwnersAsync())
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var bookService = new BookService(_bookApiClientMock.Object, Options.Create(_testConfigurationBuilder.GetBookOwnerConfig()), _loggerMock.Object);
            var result = await bookService.GetBooksAsync(bookFilter);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(errorMessage);
            result.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task GetBooksAsync_Should_ReturnNotFound_WhenNoBooksFound()
        {
            // Arrange
            var bookFilter = new BookFilter(BookTypeEnum.All);
            _bookApiClientMock.Setup(x => x.GetBookOwnersAsync())
                .ReturnsAsync(ApiResult<IEnumerable<BookOwner>>.Success(new List<BookOwner>()));

            // Act
            var bookService = new BookService(_bookApiClientMock.Object, Options.Create(_testConfigurationBuilder.GetBookOwnerConfig()), _loggerMock.Object);
            var result = await bookService.GetBooksAsync(bookFilter);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ErrorMessages.BooksNotFound);
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBooksAsync_Should_ReturnSuccessWithAllBooks_WhenBooksFound()
        {
            // Arrange
            var expectedCategorisedBooks = new CategorisedBooks();
            var expectedAdultBooks = new HashSet<string> { "Great Expectations", "Gulliver's Travels", "Hamlet", "Jane Eyre", "React: The Ultimate Guide", "Wuthering Heights" };
            var expectedChildrenBooks = new HashSet<string> { "Great Expectations", "Hamlet", "Little Red Riding Hood", "The Hobbit" };
            expectedCategorisedBooks.SetAdultBooks(expectedAdultBooks);
            expectedCategorisedBooks.SetChildrenBooks(expectedChildrenBooks);

            var expectedResult = ServiceResult<CategorisedBooks>.Success(expectedCategorisedBooks);
            var bookFilter = new BookFilter(BookTypeEnum.All);
            _bookApiClientMock.Setup(x => x.GetBookOwnersAsync())
                .ReturnsAsync(ApiResult<IEnumerable<BookOwner>>.Success(_testDataHelper.GetBookOwners()));

            // Act
            var bookService = new BookService(_bookApiClientMock.Object, Options.Create(_testConfigurationBuilder.GetBookOwnerConfig()), _loggerMock.Object);
            var result = await bookService.GetBooksAsync(bookFilter);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeEmpty();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result?.Result?.AdultBooks?.SetEquals(expectedCategorisedBooks.AdultBooks).Should().BeTrue();
            result?.Result?.ChildrenBooks?.SetEquals(expectedCategorisedBooks.ChildrenBooks).Should().BeTrue();
        }

        [Fact]
        public async Task GetBooksAsync_Should_ReturnSuccessWithHardcoverBooks_WhenFilterSetToHardcover()
        {
            // Arrange
            var expectedCategorisedBooks = new CategorisedBooks();
            var expectedAdultBooks = new HashSet<string> { "Great Expectations", "Gulliver's Travels", "Hamlet", "React: The Ultimate Guide" };
            var expectedChildrenBooks = new HashSet<string> { "Great Expectations", "Little Red Riding Hood" };
            expectedCategorisedBooks.SetAdultBooks(expectedAdultBooks);
            expectedCategorisedBooks.SetChildrenBooks(expectedChildrenBooks);

            var expectedResult = ServiceResult<CategorisedBooks>.Success(expectedCategorisedBooks);
            var bookFilter = new BookFilter(BookTypeEnum.Hardcover);
            _bookApiClientMock.Setup(x => x.GetBookOwnersAsync())
                .ReturnsAsync(ApiResult<IEnumerable<BookOwner>>.Success(_testDataHelper.GetBookOwners()));

            // Act
            var bookService = new BookService(_bookApiClientMock.Object, Options.Create(_testConfigurationBuilder.GetBookOwnerConfig()), _loggerMock.Object);
            var result = await bookService.GetBooksAsync(bookFilter);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Error.Should().BeEmpty();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result?.Result?.AdultBooks?.SetEquals(expectedCategorisedBooks.AdultBooks).Should().BeTrue();
            result?.Result?.ChildrenBooks?.SetEquals(expectedCategorisedBooks.ChildrenBooks).Should().BeTrue();
        }
    }
}
