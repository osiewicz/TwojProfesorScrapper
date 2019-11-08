using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AngleSharp.Html.Parser;
using LibScrapeTP.Entities;
using LibScrapeTP.Utilities;

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
        private const string _tpBaseUrl = @"http://twojprofesor.pl/";
        private const string _uniSubdirectories = "pracownicy/";
        private readonly string _uniUrl;

        public TpParser(University uni)
        {
            this._uniUrl = $"{_tpBaseUrl}{_uniSubdirectories}{Helpers.UniversityToUrlString(uni)}/";
        }

        public IEnumerable<Tutor> GetTutors()
        {
            foreach (var tutorPage in GetTutorPages())
            {
                yield return new Tutor();
            }
        }

        //private static string[] 
        private static HtmlParser parser = new HtmlParser();
        public IEnumerable<string> GetTutorPages()
        {
            // Returns a list of tutor pages  (as URLs) for given university.
            // The paging on destination page should be transparent for the caller and done lazily.
            using var httpClient = new HttpClient();
            // For whatever reason AngleSharp returns uppercase version of the tag.
            const string tableTag = "TABLE";
            const string tbodyTag = "TBODY";
            const string trTag = "TR";
            var it = new FragmentedPageIterator(_uniUrl);
            do
            {
                // Fetch page contents.
                var request = httpClient.GetAsync(it.Current).Result;
                if (!request.IsSuccessStatusCode)
                {
                    // We're finished.
                    yield break;
                }

                var response = request.Content.ReadAsStreamAsync().Result;

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
                    yield return $"{_tpBaseUrl}{c.Value.TrimStart('/')}";
                }
                // We've iterated through all links available on the current page.
                // Let's update searched link and go to next iteration of loop.
                it.MoveNext();
            } while (true);
        }

        private static string GetNextTutorSublist()
        {
            return string.Empty;
        }
    }
}
