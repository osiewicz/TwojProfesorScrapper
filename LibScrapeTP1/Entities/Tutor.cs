using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace LibScrapeTP.Entities
{
    [ProtoContract]
    public struct Tutor
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public AcademicTitle AcademicTitle { get; set; }
        [ProtoMember(3)]
        public string MajorDepartment { get; set; }
        [ProtoMember(4)]
        public University University { get; set; }
    }
}
