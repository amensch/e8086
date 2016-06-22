using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class OpCodeRecord
    {
        public delegate int OpCodeAction();

        public UInt16 op { get; set; }
        public int clocks { get; set; }
        public OpCodeAction opAction { get; set; }

        public OpCodeRecord(UInt16 _op, int _clocks, OpCodeAction _opAction)
        {
            op = _op;
            clocks = _clocks;
            opAction = _opAction;
        }
    }
}
