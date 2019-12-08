using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Utilities
{
    internal sealed class FragmentedPageEnumerable : IEnumerable<string>
    {
        private readonly string _baseUrl;

        public FragmentedPageEnumerable(string url)
        {
            _baseUrl = url;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new FragmentedPageIterator(_baseUrl);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
