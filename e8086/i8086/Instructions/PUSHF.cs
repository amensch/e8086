using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class PUSHF : Instruction
    {
        /// <summary>
        /// OpCode = 0x9c PUSHF
        /// Push the conditional register
        /// </summary>
        public PUSHF(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Push(EU.CondReg.Value);
        }
    }
}
