using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct TutorSummaryPage
    {
        public Tutor Tutor { get; }
        public GradeSet DisplayedGradeSet { get; }
        public Opinion[] Opinions { get; }
    }
}
