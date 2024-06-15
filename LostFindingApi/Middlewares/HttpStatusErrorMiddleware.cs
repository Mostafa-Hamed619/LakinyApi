using LostFindingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostFindingApi.Middlewares
{
    public class HttpStatusErrorMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<HttpStatusErrorMiddleware> logger;

        public HttpStatusErrorMiddleware(RequestDelegate next,ILogger<HttpStatusErrorMiddleware> logger)
        { 
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            var userName = context.User.FindFirst(ClaimTypes.GivenName)?.Value;

            httpStatusCode data = new httpStatusCode();

            var response = context.Response;
            var statusCode = response.StatusCode;
            if(statusCode == 200)
            {
               await next(context);
            }
            else
            {
                switch (statusCode)
                {
                    case 401:
                        data.Code = statusCode;
                        data.Message = "unAuthentication error";
                        Log.Error(@"{user} has made a {error} exception", userName, data.Message);
                        break;
                    case 404:
                        data.Code = statusCode;
                        data.Message = "NotFound";
                        Log.Error(@"{user} has made a {error} exception", userName, data.Message);
                        break;
                    case 400:
                        data.Code = statusCode;
                        data.Message = "Badrequest";
                        Log.Error(@"{user} has made a {error} exception", userName, data.Message);
                        break;
                    case 403:
                        data.Code = statusCode;
                        data.Message = "Forbidden";
                        Log.Error(@"{user} has made a {error} exception", userName, data.Message);
                        break;
                    case 405:
                        data.Code = statusCode;
                        data.Message = "Badrequest";
                        Log.Error(@"{user} has made a {error} exception", userName, data.Message);
                        break;
                }
            }
        }
    }
}
