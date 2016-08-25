using System.Collections.Generic;

namespace GithubReleaseHook.models
{
    public class Config
    {
        public string Repo { get; set; }
        public List<string> File { get; set; }
        public List<string> Script { get; set; }
        public string Secret { get; set; }

        public string WorkingDir { get; set; }

        public List<string> DownloadedFile { get; set; }
        public List<string> ParsedScript { get; set; }
    }
}
