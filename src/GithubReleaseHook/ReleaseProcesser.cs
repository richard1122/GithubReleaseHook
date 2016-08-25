using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GithubReleaseHook.models;

namespace GithubReleaseHook
{
    public class ReleaseProcesser
    {
        private readonly Config _config;
        private readonly ReleasePayload _payload;

        public ReleaseProcesser(Config config, ReleasePayload payload)
        {
            _config = config;
            _payload = payload;
        }

        public async Task DownloadFiles()
        {
            using (var client = new HttpClient())
            {
                _config.DownloadedFile = (await Task.WhenAll(_payload.Release.Assets
                    .FindAll(it => _config.File.Contains(it.Name))
                    .Select(async it =>
                    {
                        var tempFile = Path.GetTempFileName();
                        tempFile = Path.ChangeExtension(tempFile, it.Name.Substring(it.Name.IndexOf('.')));
                        var res = await client.GetAsync(it.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                        using (var fo = File.Create(tempFile))
                        {
                            System.Console.Write($"{it.Name} => {tempFile}: started ... ");
                            await (await res.Content.ReadAsStreamAsync()).CopyToAsync(fo);
                            System.Console.WriteLine("finished");
                        }
                        tempFile = tempFile.Replace(@"\", @"/");
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            return tempFile;
                        }
                        else
                        {
                            char c = char.ToLower(tempFile[0]);
                            tempFile = $"/mnt/{c}{tempFile.Substring(2)}";
                            return tempFile;
                        }
                    }))).ToList();
            }
        }

        public void ParseScript()
        {
            var scriptParserReg = new Regex("\\$f([0-9])");
            _config.ParsedScript = _config.Script.Select(it =>
            {
                var matches = scriptParserReg.Matches(it);
                var list = new List<int>();
                var finalPosition = 0;
                var builder = new StringBuilder();
                for (var i = 0; i != matches.Count; ++i)
                {
                    var match = matches[i];
                    var group = int.Parse(match.Groups[1].Value);
                    if (group >= _config.DownloadedFile.Count())
                        throw new IndexOutOfRangeException($"group id: {group}, DownloadedFile size: {_config.DownloadedFile.Count()}");
                    builder.Append(it, finalPosition, match.Index - finalPosition);
                    builder.Append($"\\${i}");
                    list.Add(group);
                    finalPosition = match.Index + match.Length;
                }
                builder.Append(it, finalPosition, it.Length - finalPosition);
                return $"-c \"{builder.ToString()}\" {string.Join(" ", list.Select(ith => _config.DownloadedFile[ith]))}";
            }).ToList();
        }

        public void ExecuteScript()
        {
            _config.ParsedScript.ForEach(it =>
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = it,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false
                };
                var process = new Process
                {
                    StartInfo = processStartInfo
                };
                System.Console.WriteLine(processStartInfo.Arguments);
                process.Start();
                process.WaitForExit();
            });
        }
    }   
}
