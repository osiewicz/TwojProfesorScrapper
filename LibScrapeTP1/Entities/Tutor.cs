using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct Tutor
    {
        public string Surname { get; }
        public string Name { get; }
        public AcademicTitle AcademicTitle { get; }
        public string MajorDepartment { get; }
        public string MinorDepartment { get; }
        public University University { get; }
    }
}
