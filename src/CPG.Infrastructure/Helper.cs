using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CPG.Infrastructure
{
    public static class Helper
    {

        public static string? GetClientIpAddress(this HttpRequest httpRequest)
        {
            StringValues stringValues = httpRequest.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(stringValues))
            {
                return stringValues.ToString().Split(',').Last();
            }

            return httpRequest.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }
        
    }
}
