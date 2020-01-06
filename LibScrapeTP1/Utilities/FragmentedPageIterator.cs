using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Utilities
{
    public class FragmentedPageIterator : IEnumerator<string>
    {
        public string Current { get; private set; }
        private readonly string baseUrl;
        object IEnumerator.Current => Current;
        private uint _incrementCount = 1;
        private int _fragmentIndexStartPoint = 0;

        public FragmentedPageIterator(string baseUrl)
        {
            // Assure that there's an / at the end.
            this.baseUrl = baseUrl;
            Reset();
        }

        public void Reset()
        {
            _incrementCount = 1;
            Current = baseUrl.EndsWith('/') ? baseUrl : baseUrl + '/';
            const string fragmentSpecifier = "page=";
            Current += fragmentSpecifier;
            _fragmentIndexStartPoint = Current.Length;
            MoveNext();
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            Current = Current.ToString().Substring(0, _fragmentIndexStartPoint);
            Current += _incrementCount.ToString();
            _incrementCount++;

            return true;
        }
    }
}