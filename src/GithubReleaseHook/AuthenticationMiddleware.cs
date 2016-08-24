using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace GithubReleaseHook
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRepoConfigService _repoConfigService;

        public AuthenticationMiddleware(RequestDelegate next, IRepoConfigService repoConfigService)
        {
            _next = next;
            _repoConfigService = repoConfigService;
        }

        public async Task Invoke(HttpContext context)
        {
            StringValues eventName;
            if (!context.Request.Headers.TryGetValue("X-GitHub-Event", out eventName)
                || !eventName.Equals(new[] { "release" }))
            {
                ResponseError(context);
                return;
            }
            StringValues signatureValues;
            if (!context.Request.Headers.TryGetValue("X-Hub-Signature", out signatureValues)
                || signatureValues.Count != 1)
            {
                ResponseError(context);
                return;
            }
            var signature = signatureValues[0].Split('=')[1];

            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_repoConfigService.Config.Secret));
            using (var sr = new StreamReader(context.Request.Body))
            {
                var str = await sr.ReadToEndAsync();
                context.Items["content"] = str;
                var hash = string.Concat(hmac.ComputeHash(Encoding.UTF8.GetBytes(str)).Select(it => it.ToString("X2"))).ToLower();
                if (hash != signature)
                {
                    ResponseError(context);
                    return;
                }

            }
            await _next.Invoke(context);
        }

        private void ResponseError(HttpContext context)
        {
            context.Response.StatusCode = 401;
        }
    }
}

