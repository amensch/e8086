using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal abstract class MultiplyInstruction : TwoByteInstruction
    {
        public MultiplyInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected abstract ushort GetOperand();

        // IMUL REG-16, RM-16, Imm
        protected override void ExecuteInstruction()
        {
            ushort oper1 = (ushort)GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            ushort oper2 = GetOperand();

            uint oper1ext = SignExtendWordToDW(oper1);
            uint oper2ext = SignExtendWordToDW(oper2);

            uint result = oper1ext * oper2ext;

            SaveToDestination((ushort)(result & 0xffff), direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);

            if ((result & 0xffff0000) != 0)
            {
                EU.CondReg.CarryFlag = true;
                EU.CondReg.OverflowFlag = true;
            }
            else
            {
                EU.CondReg.CarryFlag = false;
                EU.CondReg.OverflowFlag = false;
            }
        }
    }
}
