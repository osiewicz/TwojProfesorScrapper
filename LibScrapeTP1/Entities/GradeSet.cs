using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Entities
{
    // Each opinion has several opinions associated with it.
    // Additionally, each summary page contains average grades from each category
    // for a particular tutor.
    public struct GradeSet
    {
        /* Top to bottom:
         * Atrakcyjność zajęć
         * Kompetentność
         * Łatwość zaliczenia
         * Przyjazność
         * System oceniania
         * Odpracowywanie zajęć
         */
        public int AttractivenessOfClasses { get; set; }
        public int Competency { get; set; }
        public int EaseOfPassing { get; set; }
        public int Friendliness { get; set; }
        public int ScoringSystem { get; set; }
        public int AbsenceSystem { get; set; }
    }
}
