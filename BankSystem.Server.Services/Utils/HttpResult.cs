using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Utils
{
    public class HttpResult
    {
        public static class Factory
        {
            public static HttpResult Create(HttpStatusCode statusCode, object content, string errorMessage = null)
            {
                return new HttpResult
                {
                    StatusCode = (int)statusCode,
                    Content = content,
                    ErrorMessage = errorMessage
                };
            }
        }

        public int StatusCode { get; set; }
        public object Content { get; set; }
        public string ErrorMessage { get; set; }

        private HttpResult() { }
    }
}
