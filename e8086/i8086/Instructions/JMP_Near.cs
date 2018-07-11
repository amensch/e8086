using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class JMP_Near : Instruction
    {
        public JMP_Near(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort oper = GetImmediate16();
            Bus.IP += oper;
        }
    }
}
