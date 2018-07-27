using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class XLAT : Instruction
    {
        public XLAT(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // one byte instruction (0xd7)
            // Store into AL the value located at a 256 byte lookup table
            // BX is the beginning of the table and AL is the offset
            EU.Registers.AL = (byte)(Bus.GetData(0, EU.Registers.BX + EU.Registers.AL));
        }
    }
}
