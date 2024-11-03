using System.Net;

namespace Core.Models
{
    public class ServiceResult<T>
    {
        private ServiceResult(bool isSuccess, int statusCode, T? result, string? error)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Result = result;
            Error = error;
        }

        public bool IsSuccess { get; private set; }

        public int StatusCode { get; private set; }

        public T? Result { get; private set; }

        public string? Error { get; private set; }

        public static ServiceResult<T> Success(T? result) => new(true, (int)HttpStatusCode.OK, result, string.Empty);

        public static ServiceResult<T> Failure(HttpStatusCode statusCode, string error) => new(false, (int)statusCode, default, error);
    }
}
