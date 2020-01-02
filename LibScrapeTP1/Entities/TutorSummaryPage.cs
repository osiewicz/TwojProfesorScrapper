using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct TutorSummaryPage
    {
        public Tutor Tutor { get; set; }
        public GradeSet DisplayedGradeSet { get; set; }
        public Opinion[] Opinions { get; set; }
    }
}
