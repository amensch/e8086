using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086Disassembler
{
    public class OpCodeRecord
    {
        public delegate void OpCodeAction();

        public OpCodeAction opAction { get; set; }

        public OpCodeRecord(OpCodeAction _opAction)
        {
            opAction = _opAction;
        }
    }
}
