using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            try
            {
                var client = new HttpClient();
                var files = await Task.WhenAll(_payload.Release.Assets
                    .FindAll(it => _config.File.Contains(it.Name))
                    .Select(async it =>
                    {
                        var tempFile = Path.GetTempFileName();
                        var res = await client.GetAsync(it.BrowserDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                        using (var fo = File.OpenWrite(tempFile))
                        {
                            System.Console.Write($"{it.Name} => {tempFile}: started ... ");
                            await (await res.Content.ReadAsStreamAsync()).CopyToAsync(fo);
                            System.Console.WriteLine("finished");
                        }
                        return tempFile;
                    }));
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex.StackTrace);
                throw;
            }
            
        }
    }
}
