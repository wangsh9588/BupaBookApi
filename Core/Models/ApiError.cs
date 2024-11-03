using System.Net;

namespace Core.Models
{
    public class ApiError
    {
        private ApiError(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string Message { get; private set; } = string.Empty;

        public static ApiError None() => new(HttpStatusCode.OK, string.Empty);

        public static ApiError Create(HttpStatusCode statusCode, string message) => new(statusCode, message);

        public static ApiError BadRequest(string message) => new(HttpStatusCode.BadRequest, message);
    }
}
