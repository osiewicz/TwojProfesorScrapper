using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibScrapeTP.ETLSteps.Transform
{
    internal static class TitleParser
    {
        private static readonly Dictionary<string, AcademicTitle> abbreviatedTitleToAcademicTitle = new Dictionary<string, AcademicTitle>()
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
            var highestTitle = abbreviatedTitleToAcademicTitle.Keys.Where(text.Contains).Last();

            return abbreviatedTitleToAcademicTitle[highestTitle];
        }
    }
}
