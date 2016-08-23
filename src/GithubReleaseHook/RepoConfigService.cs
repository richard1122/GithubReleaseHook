using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GithubReleaseHook
{
    public interface IRepoConfigService
    {
        Config GetConfig();
    }
    public class RepoConfigService:IRepoConfigService
    {
        private Config config;
        public RepoConfigService()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "repo.yml");
            var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
//            config = deserializer.Deserialize<Config>(File.OpenText(path));
//            File.OpenText(path).Dispose();
            var watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), "repo.yml") {EnableRaisingEvents = true};
            watcher.Changed += (sender, args) =>
            {
                config = deserializer.Deserialize<Config>(File.OpenText(path));
            };
        }

        Config IRepoConfigService.GetConfig()
        {
            throw new NotImplementedException();
        }

    }
}
