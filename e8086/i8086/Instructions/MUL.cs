using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class MUL : TwoByteInstruction
    {
        public MUL(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int result = 0;
            if (wordSize == 0)
            {
                result = source * EU.Registers.AL;
                EU.Registers.AX = (ushort)result;

                EU.CondReg.CarryFlag = (EU.Registers.AH != 0);
                EU.CondReg.OverflowFlag = EU.CondReg.CarryFlag;
            }
            else
            {
                result = source * EU.Registers.AX;
                EU.Registers.DX = (ushort)(result >> 16);
                EU.Registers.AX = (ushort)(result);

                EU.CondReg.CarryFlag = (EU.Registers.DX != 0);
                EU.CondReg.OverflowFlag = EU.CondReg.CarryFlag;
            }
        }
    }
}
