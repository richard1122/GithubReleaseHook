using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GithubReleaseHook
{
    public class Server
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRepoConfigService, RepoConfigService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var configService = context.RequestServices.GetService(typeof(IRepoConfigService));
                await context.Response.WriteAsync("Hello");
            });
        }
    }
}
