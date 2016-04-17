using System;
using System.Collections.Generic;

namespace OrgPlay.Organya
{
    public class OrganyaInstrument
    {
        public short FineTune { get; set; }
        public byte InstrumentIndex { get; set; }
        public bool DisableSustain { get; set; } // 'Pi' value.
        public List<OrganyaNote> Notes { get; set; }
    }
}

