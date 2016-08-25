using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GithubReleaseHook.models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GithubReleaseHook
{
    public interface IRepoConfigService
    {
        Config Config { get; }
    }
    public class RepoConfigService : IRepoConfigService
    {
        public Config Config { get; }
        private readonly object _thisLock = new object();

        public RepoConfigService()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "repo.yml");
            var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention(), ignoreUnmatched: true);
            using (var sr = File.OpenText(path))
            {
                Config = deserializer.Deserialize<Config>(sr);
            }
            Config.WorkingDir = Utils.ConvertPathToCrossPlatform(
                Path.IsPathRooted(Config.WorkingDir)
                    ? Config.WorkingDir
                    : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), Config.WorkingDir)));
        }
    }
}
