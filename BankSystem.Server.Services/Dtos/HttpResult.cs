using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Server.Services.Dtos
{
    public class HttpResult<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        public static HttpResult<T> Success(T data, string message = "Success", int statusCode = 200)
        {
            return new HttpResult<T> { Data = data, Message = message, StatusCode = statusCode };
        }

        public static HttpResult<T> Fail(string message, int statusCode = 400)
        {
            return new HttpResult<T> { Message = message, StatusCode = statusCode };
        }
    }

}
