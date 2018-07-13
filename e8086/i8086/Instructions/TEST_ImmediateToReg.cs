using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class TEST_ImmediateToReg : TwoByteInstruction
    {
        public TEST_ImmediateToReg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int dest = 0;
            if (wordSize == 0)
            {
                dest = Bus.NextIP();
            }
            else
            {
                dest = GetImmediateWord();
            }
            int result = source & dest;
            EU.CondReg.OverflowFlag = false;
            EU.CondReg.CarryFlag = false;
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcParityFlag(result);
        }

    }
}
