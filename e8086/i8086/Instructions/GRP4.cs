using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class GRP4 : GroupInstruction
    {
        public GRP4(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void LoadInstructionList()
        {
            // REG 000: INC reg/mem-8
            // REG 001: DEC reg/mem-8
            // all others are undefined

            instructions.Add(0x00, new INC_RegMem(OpCode, EU, Bus));
            instructions.Add(0x01, new DEC_RegMem(OpCode, EU, Bus));

        }
    }
}
