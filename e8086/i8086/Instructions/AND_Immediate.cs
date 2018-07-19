using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class AND_Immediate : LogicalImmediate
    {
        public AND_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ProcessInstruction(source, direction, wordSize, 0x03, 0x00, 0x00, false);
        }

        protected override int Operand(int source, int dest)
        {
            return (dest & source);
        }
    }
}
