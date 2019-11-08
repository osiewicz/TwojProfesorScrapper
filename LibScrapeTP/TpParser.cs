using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using LibScrapeTP.Entities;

namespace LibScrapeTP
{
    // 
    public enum Title
    {
        Engineer,
        Master,
    }

    public class TpParser
    {
        private static string _tpBaseUrl = @"http://twojprofesor.pl/pracownicy/";
        private string _uniUrl;
        public TpParser(University uni)
        {
            this._uniUrl = $"{_tpBaseUrl}{Helpers.UniversityToUrlString(uni)}/";
        }
        public IEnumerable<Tutor> GetTutors()
        {
            foreach(var tutorPage in GetTutorPages())
            {
                yield return new Tutor();
            }
        }
        
        //private static string[] 
        private async IAsyncEnumerator<string> GetTutorPages()
        {
            // Returns a list of tutor pages  (as URLs) for given university.
            // The paging on destination page should be transparent for the caller.
            var currentUrl = _uniUrl;
            var httpClient = new HttpClient();
            var parser = new HtmlParser();
            do
            {
                // Fetch page contents.
                var request = await httpClient.GetAsync(currentUrl);
                var response = await request.Content.ReadAsStreamAsync();

                IHtmlDocument document = parser.ParseDocument(response);
                document.All.Where(x=> x.HasChildNodes && new []{"table", "table-bordered", "table-hover"}.All(x.ClassList.Contains))

            } while (true);

            yield break;
        }

        private static string GetNextTutorSublist()
        {
            return string.Empty;
        }
    }


}
