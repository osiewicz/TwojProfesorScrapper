using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Utilities
{
    class FragmentedPageIterator
    {
        public string Current { get; private set; }
        private uint _incrementCount = 1;
        private int _fragmentIndexStartPoint = 0;
        public FragmentedPageIterator(string baseUrl)
        {
            // Assure that there's an / at the end.
            Current = baseUrl.EndsWith('/')? baseUrl: baseUrl + '/';
            const string fragmentSpecifier = "page=";
            Current += fragmentSpecifier;
            _fragmentIndexStartPoint = Current.Length;
            MoveNext();
        }

        public void MoveNext()
        {
            Current = Current.Substring(0, _fragmentIndexStartPoint);
            Current += _incrementCount.ToString();
            _incrementCount++;
        }
    }
}
