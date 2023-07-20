using System.Net;

namespace Timecoder.Network.Models
{
    internal class ApiResponse
    {
        public bool IsSuccess { get; }
        public HttpStatusCode StatusCode { get; }
        public string ResponseData { get; }
        public string ErrorMessage { get; }

        public ApiResponse(bool isSuccess, HttpStatusCode statusCode, string responseData, string errorMessage)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            ResponseData = responseData;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            return $"IsSuccess: {IsSuccess}, StatusCode: {StatusCode}, ResponseData: {ResponseData}, ErrorMessage: {ErrorMessage}";
        }
    }
}
