using System;
using System.Collections.Generic;
using System.Text;

namespace LibScrapeTP.Entities
{
    public struct Tutor
    {
        public string Name { get; set; }
        public AcademicTitle AcademicTitle { get; set; }
        public string MajorDepartment { get; set; }
        public University University { get; set; }
    }
}
