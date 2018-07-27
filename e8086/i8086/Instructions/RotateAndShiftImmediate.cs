using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class RotateAndShiftImmediate : RotateAndShift
    {
        public RotateAndShiftImmediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override int GetSourceData()
        {
            // in this case we need to read the RM data first in the case there
            // is displacement data to read before the immediate data
            int operand1 = GetSourceData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            return Bus.NextImmediate();
        }
    }
}
