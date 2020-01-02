using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct Opinion
    {
        public string Name { get; set; }
        public GradeSet Grades { get; set; }
        public DateTime AddedOn { get; set; }
        public string Subject { get; set; }
        public string Comment { get; set; }
    }
}
