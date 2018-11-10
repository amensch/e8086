using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CALL_Near : Instruction
    {
        public CALL_Near(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort oper = GetImmediateWord();
            Push(Bus.IP);
            Bus.IP += oper;
        }

        protected override void DetermineClocks()
        {
            Clocks = 19;
        }
    }
}
