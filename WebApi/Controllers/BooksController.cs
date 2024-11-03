using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService ?? throw new ArgumentException(nameof(bookService));
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] BookTypeEnum? bookType)
        {
            if (ModelState.IsValid)
            {
                var filter = new BookFilter(bookType.GetValueOrDefault());
                var response = await _bookService.GetBooksAsync(filter).ConfigureAwait(false);
                return StatusCode(response.StatusCode, response);
            }
            else
            {
                return BadRequest(ApiResult.Failure(ApiError.BadRequest(string.Join(";", ModelState.Values.SelectMany(v => v.Errors)))));
            }
        }
    }
}
