using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GithubReleaseHook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://+:8081")
                .UseStartup<Server>()
                .Build();
            host.Run();
        }
    }
}
