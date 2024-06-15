using LostFindingApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostFindingApi.Middlewares
{
    public class userInfoMiddlewar
    {
        private readonly RequestDelegate next;
        private readonly ILogger<userInfoMiddlewar> logger;
        private static int  count = 0;
        public userInfoMiddlewar(RequestDelegate next,ILogger<userInfoMiddlewar> logger)
        {
            this.next = next;
            this.logger = logger;
            
        }

        public async Task Invoke(HttpContext context)
        {
            var user = context.User.FindFirst(ClaimTypes.GivenName)?.Value;

            
            DateTime dateTime = DateTime.Now;
            TimeSpan timeOnly = dateTime.TimeOfDay;
            await next(context);
            count++;
            if (user == null)
            {
                Log.Information("some has get to server {time} for {action} with {count}", timeOnly, context.Request.Path, count);
            }
            else
            {
                Log.Information($"{user} has get to the server at {timeOnly} to the action {context.Request.Path} with request {count}");
            }
        }
    }
}
