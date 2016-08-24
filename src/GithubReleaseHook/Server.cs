using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GithubReleaseHook.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            app.UseMiddleware<AuthenticationMiddleware>();
            app.Run(async context =>
            {
                var configService = context.RequestServices.GetService(typeof(IRepoConfigService)) as IRepoConfigService;
                await context.Response.WriteAsync("Hello");
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var jsonSerializerSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
                    var str = context.Items["content"] as string;
                    var payload = JsonConvert.DeserializeObject<ReleasePayload>(str, jsonSerializerSettings);
                    var processer = new ReleaseProcesser(configService.Config, payload);
                    System.Console.WriteLine("Start downloading files:");
                    await processer.DownloadFiles();
                }
            });
        }
    }
}
