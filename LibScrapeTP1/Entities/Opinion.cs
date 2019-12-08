using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct Opinion
    {
        public string Name { get; }
        public GradeSet Grades { get; }
        public DateTime AddedOn { get; }
        [MaybeNull]
        public string Subject { get; }
        public int OpinionRating { get; }
    }
}
