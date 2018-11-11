using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class JMP_Near : Instruction
    {
        public JMP_Near(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort oper = GetImmediateWord();
            Bus.IP += oper;
        }

        protected override void DetermineClocks()
        {
            Clocks = 15;
        }
    }
}
