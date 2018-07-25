using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class NEG : TwoByteInstruction
    {
        public NEG(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int dest = 0;
            int result = (~source) + 1;
            SaveToDestination(result, 0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            EU.CondReg.CalcOverflowFlag(wordSize, 0, result);
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcAuxCarryFlag(source, dest);
            EU.CondReg.CalcParityFlag(result);
            EU.CondReg.CalcCarryFlag(wordSize, result);
        }
    }
}
