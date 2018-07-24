using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class InvalidInstruction : Instruction
    {
        public InvalidInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            throw new InvalidOperationException("Instruction " + OpCode.ToString("X2") + " is not implemented");
        }
    }
}
