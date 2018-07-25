using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class POPA : Instruction
    {
        public POPA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            EU.Registers.DI = Pop();
            EU.Registers.SI = Pop();
            EU.Registers.BP = Pop();
            ushort new_sp = Pop();
            EU.Registers.BX = Pop();
            EU.Registers.DX = Pop();
            EU.Registers.CX = Pop();
            EU.Registers.AX = Pop();
        }
    }
}
