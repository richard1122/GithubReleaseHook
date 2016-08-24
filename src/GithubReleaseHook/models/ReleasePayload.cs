using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GithubReleaseHook.models
{
    public class Repository
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        public string Name { get; set; }
    }

    public class Release
    {
        public string Url { get; set; }
        [JsonProperty("tag_name")]
        public string TagName { get; set; }
        public bool Prerelease { get; set; }
        public List<Asset> Assets { get; set; }
    }

    public class Asset
    {
        public string Url { get; set; }
        public string Name { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        public int Size { get; set; }
        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }

    public class ReleasePayload
    {
        public Release Release { get; set; }
        public Repository Repository { get; set; }
    }
}
