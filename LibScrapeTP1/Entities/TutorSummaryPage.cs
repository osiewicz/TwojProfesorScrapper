using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace LibScrapeTP.Entities
{
    [ProtoContract]
    public struct TutorSummaryPage
    {
        [ProtoMember(1)]
        public Tutor Tutor { get; set; }
        [ProtoMember(2)]
        public GradeSet DisplayedGradeSet { get; set; }
        [ProtoMember(3)]
        public Opinion[] Opinions { get; set; }
    }
}
