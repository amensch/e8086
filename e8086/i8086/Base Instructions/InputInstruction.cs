using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal abstract class InputInstruction : Instruction
    {
        protected ushort port = 0;
        public InputInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            if(wordSize == 0)
            {
                EU.Registers.AL = (byte)EU.ReadPort(wordSize, port);
            }
            else
            {
                EU.Registers.AX = (ushort)EU.ReadPort(wordSize, port);
            }
        }
    }
}
