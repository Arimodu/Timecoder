using System.Net;

namespace Timecoder.Network.Models
{
    internal class ApiResponse
    {
        public bool IsSuccess { get; }
        public int StatusCode { get; }
        public string ResponseData { get; }
        public string ErrorMessage { get; }

        public ApiResponse(bool isSuccess, int statusCode, string responseData, string errorMessage)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            ResponseData = responseData;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            if (IsSuccess) return $"Success: true\nStatus Code: {StatusCode}\nResponse: {ResponseData}";
            return $"IsSuccess: false\nStatusCode: {StatusCode}\nResponseData: {ResponseData}\nErrorMessage: {ErrorMessage}";
        }
    }
}
