using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibScrapeTP.ETLSteps.Transform
{
    internal static class TitleParser
    {
        private static readonly Dictionary<string, AcademicTitle> AbbreviatedTitleToAcademicTitle = new Dictionary<string, AcademicTitle>()
        {
            {"inż", AcademicTitle.Engineer},
            {"mgr", AcademicTitle.Master },
            {"dr", AcademicTitle.PhD },
            {"dr hab", AcademicTitle.Pdd },
            {"prof", AcademicTitle.Profesor }
        };
        // Takes a full title (example: mgr inż. dr hab.) and extracts the highest possible title from it as enum.
        public static AcademicTitle Parse(string text)
        {
            // Dictionary keys are not guaranteed to be stored in the same order as in initializer.
            var highestTitle = AbbreviatedTitleToAcademicTitle.Where(title=>text.Contains(title.Key)).Max(kv => kv.Value);

            return highestTitle;
        }
    }
}
