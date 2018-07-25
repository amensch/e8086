using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// OpCode 0x68: PUSH Imm-16
    /// </summary>
    internal class PUSH_ImmediateWord : Instruction
    {
        public PUSH_ImmediateWord(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Push(GetImmediateWord());
        }
    }
}
