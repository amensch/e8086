using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class IMUL_ImmediateByte : MultiplyInstruction
    {
        public IMUL_ImmediateByte(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override ushort GetOperand()
        {
            return Bus.NextIP();
        }
    }
}
