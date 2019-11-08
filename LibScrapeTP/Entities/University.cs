using System.Collections.Generic;

namespace LibScrapeTP.Entities
{
    public enum University
    {
    }

    public static partial class Helpers
    {
        private static Dictionary<University, string> uniToString = new Dictionary<University, string>();
        public static string UniversityToUrlString(University uni)
        {
            return uniToString[uni];
        }
    }
}
