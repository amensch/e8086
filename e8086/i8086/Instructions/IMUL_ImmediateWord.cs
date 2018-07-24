using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class IMUL_ImmediateWord : MultiplyInstruction
    {
        public IMUL_ImmediateWord(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override ushort GetOperand()
        {
            return GetImmediateWord();
        }
    }
}
