namespace Core.Models
{
    public class ApiResult
    {
        protected ApiResult(bool isSuccess, ApiError error) 
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; private set; }

        public ApiError Error { get; private set; }

        public static ApiResult Success() => new(true, ApiError.None());

        public static ApiResult Failure(ApiError error) => new(false, error);
    }

    public class ApiResult<T> : ApiResult
    {
        protected ApiResult(bool isSuccess, ApiError error, T? data)
            : base(isSuccess, error)
        {
            Data = data;
        }

        public T? Data { get; private set; }

        public static ApiResult<T> Success(T? data) => new(true, ApiError.None(), data);

        public static new ApiResult<T> Failure(ApiError error) => new(false, error, default);
    }
}
