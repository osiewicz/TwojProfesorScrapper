using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct Opinion
    {
        public string Name { get; set; }
        public GradeSet Grades { get; }
        public DateTime AddedOn { get; set; }
        [MaybeNull]
        public string Subject { get; set; }
    }
}
