using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GithubReleaseHook
{
    public class Config
    {
        public string Repo { get; set; }
        public List<string> File { get; set; }
        public List<string> Script { get; set; }
        public string Secret { get; set; }

        public List<string> DownloadedFile { get; set; }
        public List<string> ParsedScript { get; set; }
    }
}
