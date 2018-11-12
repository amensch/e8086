using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class NOOP : Instruction
    {
        public NOOP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        public override long Clocks()
        {
            return 3;
        }
    }
}
