using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ProtoBuf;

namespace LibScrapeTP.Entities
{
    [ProtoContract]
    public struct Opinion
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public GradeSet Grades { get; set; }
        [ProtoMember(3)]
        public DateTime AddedOn { get; set; }
        [ProtoMember(4)]
        public string Subject { get; set; }
        [ProtoMember(5)]
        public string Comment { get; set; }
    }
}
