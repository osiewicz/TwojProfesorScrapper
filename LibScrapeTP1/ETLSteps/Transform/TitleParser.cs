using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.ETLSteps.Transform
{
    internal static class TitleParser
    {
        public static AcademicTitle Parse(string text)
        {
            return AcademicTitle.Engineer;
        }
    }
}
