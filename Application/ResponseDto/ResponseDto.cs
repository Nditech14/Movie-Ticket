using System.Net;

namespace Application.ResponseDto
{
    public class ResponseDto<T>
    {
        public bool IsSuccessful { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public IEnumerable<Error> Errors { get; set; }

        public ResponseDto()
        {
        }

        public ResponseDto(T? data, string message, bool isSuccessful, int statusCode, IEnumerable<Error> errors)
        {
            IsSuccessful = isSuccessful;
            Code = statusCode;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static ResponseDto<T> Failure(IEnumerable<Error> errors, int statusCode = (int)HttpStatusCode.BadRequest)
        {
            return new ResponseDto<T>(default, string.Empty, false, statusCode, errors);
        }

        public static ResponseDto<T> Success(T data, string successMessage = "", int statusCode = (int)HttpStatusCode.OK)
        {
            return new ResponseDto<T>(data, successMessage, true, statusCode, Array.Empty<Error>());
        }

        public static ResponseDto<T> Success(string successMessage = "", int statusCode = (int)HttpStatusCode.OK)
        {
            return new ResponseDto<T>(default, successMessage, true, statusCode, Array.Empty<Error>());
        }
    }

    public class Error
    {
        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Message { get; }
        public string Code { get; }

        public static readonly IEnumerable<Error> None = Array.Empty<Error>();
        public static readonly Error NullValue = new("Error.NullValue", "the specified result value is null.");
    }
}