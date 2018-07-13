using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class NOT_Immediate : TwoByteInstruction
    {
        public NOT_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            SaveToDestination(~source, 0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
        }
    }
}
