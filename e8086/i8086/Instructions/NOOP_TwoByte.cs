using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class NOOP_TwoByte : TwoByteInstruction
    {
        public NOOP_TwoByte(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
    }
}
