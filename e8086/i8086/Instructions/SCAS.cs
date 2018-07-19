using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class SCAS : RepeatableInstruction
    {
        public SCAS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            int source = 0;
            int dest = Bus.GetDestString(wordSize, EU.Registers.DI);

            if (wordSize == 0)
            {
                source = EU.Registers.AL;
                if (EU.CondReg.DirectionFlag)
                {
                    EU.Registers.DI--;
                }
                else
                {
                    EU.Registers.DI++;
                }
            }
            else
            {
                source = EU.Registers.AX;
                if (EU.CondReg.DirectionFlag)
                {
                    EU.Registers.DI -= 2;
                }
                else
                {
                    EU.Registers.DI += 2;
                }
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

    }
}
