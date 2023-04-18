using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace backend.Errors
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }

        public ApiException(HttpResponseMessage response)
        {
            Response = response;
        }

        public ApiException(HttpResponseMessage response, string message)
            : base(message)
        {
            Response = response;
        }

        public ApiException(HttpResponseMessage response, string message, Exception innerException)
            : base(message, innerException)
        {
            Response = response;
        }
    }
}
