﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GithubReleaseHook.models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
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
                try
                {
                    StringValues id;
                    context.Request.Headers.TryGetValue("X-GitHub-Delivery", out id);
                    System.Console.WriteLine($"Start processing delivery {id[0]}");
                    var configService = context.RequestServices.GetService(typeof(IRepoConfigService)) as IRepoConfigService;
                    await context.Response.WriteAsync("Hello");
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                        var str = context.Items["content"] as string;
                        var payload = JsonConvert.DeserializeObject<ReleasePayload>(str, jsonSerializerSettings);
                        var processer = new ReleaseProcesser(configService.Config, payload);
                        System.Console.WriteLine("Start downloading files:"); 
                        await processer.DownloadFiles();
                        processer.ParseScript();
                        await processer.ExecuteScript();
                    }
                    System.Console.WriteLine($"delivery {id[0]} process finished.");
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine(ex.StackTrace);
                    throw;
                }
                
            });
        }
    }
}
