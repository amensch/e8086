using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class SCAS : RepeatableInstruction
    {
        public SCAS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            int source = 0;
            int dest = Bus.GetDestString(wordSize, EU.Registers.DI);

            if (EU.CondReg.DirectionFlag)
            {
                EU.Registers.DI -= (ushort)(wordSize + 1);
            }
            else
            {
                EU.Registers.DI += (ushort)(wordSize + 1);
            }

            if (wordSize == 0)
            {
                source = EU.Registers.AL;
            }
            else
            {
                source = EU.Registers.AX;
            }

            int result = dest - source;
            EU.CondReg.CalcOverflowFlag(wordSize, source, dest);
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcAuxCarryFlag(source, dest);
            EU.CondReg.CalcParityFlag(result);
            EU.CondReg.CalcCarryFlag(wordSize, result);
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
            if (EU.RepeatMode == RepeatModeEnum.NoRepeat)
            {
                return 15;
            }
            else
            {
                return RepeatCount * 15;
            }
        }

    }
}
