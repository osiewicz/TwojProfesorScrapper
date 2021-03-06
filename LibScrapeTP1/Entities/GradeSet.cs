﻿using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace LibScrapeTP.Entities
{
    // Each opinion has several opinions associated with it.
    // Additionally, each summary page contains average grades from each category
    // for a particular tutor.
    [ProtoContract]
    public struct GradeSet
    {
        /* Top to bottom:
         * Atrakcyjność zajęć
         * Kompetentność
         * Łatwość zaliczenia
         * Przyjazność
         * System oceniania
         * Odpracowywanie zajęć
         */
        [ProtoMember(1)]
        public short AttractivenessOfClasses { get; set; }
        [ProtoMember(2)]
        public short Competency { get; set; }
        [ProtoMember(3)]
        public short EaseOfPassing { get; set; }
        [ProtoMember(4)]
        public short Friendliness { get; set; }
        [ProtoMember(5)]
        public short ScoringSystem { get; set; }
        [ProtoMember(6)]
        public short AbsenceSystem { get; set; }
    }
}
