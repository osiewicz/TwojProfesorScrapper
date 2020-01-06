using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using LibScrapeTP.Entities;
using LibScrapeTP.Utilities;

namespace LibScrapeTP.ETLSteps.Extract
{
   /// <summary>
   /// TpEPageExtracter handles obtaining urls of pages that
   /// should be downloaded into local cache.
   /// </summary>
    public class PageUrlExtracter
    {
        private const string _tpBaseUrl = @"http://twojprofesor.pl/";
        private const string _uniSubdirectories = "pracownicy/";
        private readonly string _uniUrl;
        private readonly TimeSpan _timeout;

        public PageUrlExtracter(University uni, TimeSpan fetchTimeout)
        {
            _uniUrl = $"{_tpBaseUrl}{_uniSubdirectories}{Helpers.UniversityToUrlString(uni)}/";
            _timeout = fetchTimeout;
        }

        public IEnumerable<string> GetTutorPagesUrls()
        {
            // Returns a list of tutor pages  (as URLs) for given university.
            // The paging on destination page should be transparent for the caller and done lazily.
            
            // For whatever reason AngleSharp returns uppercase version of the tag.
            const string tableTag = "TABLE";
            const string tbodyTag = "TBODY";
            const string trTag = "TR";
            var parser = new HtmlParser();
            var listPages = new BlockingCollection<string>();
            var fillQueueTask = Task.Run(() => GetTutorListPages(listPages, _uniUrl, _timeout));

            var result = new ConcurrentBag<string>();

            // TODO: Refactor into Producer/Consumer pattern.
            Parallel.ForEach(listPages.GetConsumingEnumerable(), response =>
            {
                var document = parser.ParseDocument(response);
                var table = document.All.First(x => x.TagName.Equals(tableTag) &&
                                                    x.HasChildNodes &&
                                                    new[] {"table", "table-bordered", "table-hover"}.All(x.ClassList
                                                        .Contains));
                foreach (var child in table.Children.FirstOrDefault(child => child.TagName.Equals(tbodyTag))?.Children
                    .Where(row => row.TagName.Equals(trTag)))
                {
                    var linkToPage = child.Children.First().Children.First();
                    var c = linkToPage.Attributes.GetNamedItem("href");
                    result.Add($"{_tpBaseUrl}{c.Value.TrimStart('/')}");
                }
            });

            fillQueueTask.Wait(); // Should be done at this point.

            return result;
        }

        private static void GetTutorListPages(BlockingCollection<string> toFill, string url, TimeSpan timeout)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:71.0) Gecko/20100101 Firefox/71.0");
            var responses = new List<HttpResponseMessage>();
            foreach (var it in new FragmentedPageEnumerable(url))
            {
                var request = httpClient.GetAsync(it);
                var completedWithinTimeout = request.Wait(timeout);
                if (!completedWithinTimeout)
                {
                    // non-fatal.
                    continue;
                }

                if (!request.Result.IsSuccessStatusCode)
                {
                    // We're finished.
                    break;
                }

                responses.Add(request.Result);
            }

            Parallel.ForEach(responses, request =>
            {
                var response = request.Content.ReadAsStringAsync().Result;
                toFill.Add(response);
            });

            toFill.CompleteAdding();
        }
    }
}
