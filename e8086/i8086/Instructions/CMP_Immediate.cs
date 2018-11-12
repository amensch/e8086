using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CMP_Immediate : SUB_Immediate
    {
        public CMP_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            SubWithBorrow = false;
            CompareOnly = true;
        }

        public override long Clocks()
        {
            // acc,imm
            return 4;
        }
    }
}
