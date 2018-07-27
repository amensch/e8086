using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class XCHG_AX : Instruction
    {
        public XCHG_AX(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // Op Codes: 90-97
            // note 90 is XCHG AX,AX and thus is a NOP

            // Parse the op code, last 3 bits indicates register.
            // All swaps are done with AX

            int first = EU.Registers.GetRegisterValue(1, OpCodeMode.RM);
            EU.Registers.SaveRegisterValue(1, OpCodeMode.RM, EU.Registers.AX);
            EU.Registers.AX = (ushort)first;

            // no flags are affected
        }
    }
}
