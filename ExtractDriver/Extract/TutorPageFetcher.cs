using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibScrapeTP.ETLSteps.Extract
{
    public class TutorPageFetcher
    {
        private readonly string _fsPath;
        public TutorPageFetcher(string fsTargetPath)
        {
            _fsPath = fsTargetPath;
        }

        private static readonly SHA1 Sha = SHA1.Create();
        private static readonly Func<string, string> NameProvider = str => BitConverter.ToString(Sha.ComputeHash(Encoding.ASCII.GetBytes(str)));
        // Remove any "-" character that are present by default in BitConverter.ToString output.
        public void GetPages(List<string> tutorPageUrls, TimeSpan pageFetchTimeout)
        {
            if (!Directory.Exists(_fsPath))
            {
                Directory.CreateDirectory(_fsPath);
            }

            using var http = new HttpClient();
            Parallel.ForEach(tutorPageUrls, url =>
            {
                var pageFetch = http.GetAsync(url);
                var completedWithinTimeout = pageFetch.Wait(pageFetchTimeout);
                if (!completedWithinTimeout)
                {
                    return;
                }
                var page = pageFetch.Result;
                Debug.Assert(page.IsSuccessStatusCode);
                if (!page.IsSuccessStatusCode)
                {
                    // Skip this page.
                    return;
                }

                var pageContent = page.Content.ReadAsStreamAsync();
                var contentAsString = page.Content.ReadAsStringAsync();
                var pathName = Path.Combine(_fsPath, NameProvider(contentAsString.Result));
                using var outputFile = File.Create(pathName);
                pageContent.Result.CopyTo(outputFile);
            });
        }
    }
}
