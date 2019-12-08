using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibScrapeTP.ETLSteps.Extract
{
    internal class TutorPageFetcher
    {
        private readonly string _fsPath;
        public TutorPageFetcher(string fsTargetPath)
        {
            _fsPath = fsTargetPath;
        }

        private static readonly SHA1 Sha = SHA1.Create();
        private static readonly Func<string, string> NameProvider = str => Sha.ComputeHash(Encoding.ASCII.GetBytes(str)).ToString();
        public void GetPages(List<string> tutorPageUrls)
        {
            if (!Directory.Exists(_fsPath))
            {
                Directory.CreateDirectory(_fsPath);
            }

            using var http = new HttpClient();
            Parallel.ForEach(tutorPageUrls, url =>
            {
                var page = http.GetAsync(url).Result;

                Debug.Assert(page.IsSuccessStatusCode);
                if (!page.IsSuccessStatusCode)
                {
                    // Skip this page.
                    return;
                }

                var pageContent = page.Content.ReadAsStreamAsync().Result;
                var pathName = Path.Combine(_fsPath, NameProvider(url));
                using var outputFile = File.Create(pathName);
                pageContent.CopyTo(outputFile);
            });
        }
    }
}
