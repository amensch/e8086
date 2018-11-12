using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CMPS : RepeatableInstruction
    {
        public CMPS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            int source = Bus.GetDestString(wordSize, EU.Registers.DI);
            int dest = Bus.GetData(wordSize, EU.Registers.SI);
            int result = dest - source;

            EU.CondReg.CalcOverflowFlag(wordSize, source, dest);
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcAuxCarryFlag(source, dest);
            EU.CondReg.CalcParityFlag(result);
            EU.CondReg.CalcCarryFlag(wordSize, result);

            if (EU.CondReg.DirectionFlag)
            {
                EU.Registers.SI -= (ushort)(wordSize + 1);
                EU.Registers.DI -= (ushort)(wordSize + 1);
            }
            else
            {
                EU.Registers.SI += (ushort)(wordSize + 1);
                EU.Registers.DI += (ushort)(wordSize + 1);
            }
        }

        protected override bool RepeatConditions()
        {
            // REP is repeat while zero
            if (EU.RepeatMode == RepeatModeEnum.REP)
                return EU.CondReg.ZeroFlag;

            // REPNZ is repeat while not zero
            else if (EU.RepeatMode == RepeatModeEnum.REPNZ)
                return !EU.CondReg.ZeroFlag;

            // otherwise no repeat
            else
                return false;
        }

        public override long Clocks()
        {
            return RepeatCount * 22;
        }

    }
}
