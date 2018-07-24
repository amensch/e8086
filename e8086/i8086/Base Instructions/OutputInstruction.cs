using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal abstract class OutputInstruction : Instruction
    {
        protected ushort port = 0;
        public OutputInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            if(wordSize == 0)
            {
                EU.WritePort(wordSize, port, EU.Registers.AL);
            }
            else
            {
                EU.WritePort(wordSize, port, EU.Registers.AX);
            }
        }
    }
}
