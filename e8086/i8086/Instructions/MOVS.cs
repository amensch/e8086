using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class MOVS : RepeatableInstruction
    {
        public MOVS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            if(wordSize == 0)
            {
                Bus.MoveByteString(EU.Registers.SI, EU.Registers.DI);
                if(EU.CondReg.DirectionFlag)
                {
                    EU.Registers.SI--;
                    EU.Registers.DI--;
                }
                else
                {
                    EU.Registers.SI++;
                    EU.Registers.DI++;
                }
            }
            else
            {
                Bus.MoveWordString(EU.Registers.SI, EU.Registers.DI);
                if (EU.CondReg.DirectionFlag)
                {
                    EU.Registers.SI -= 2;
                    EU.Registers.DI -= 2;
                }
                else
                {
                    EU.Registers.SI += 2;
                    EU.Registers.DI += 2;
                }
            }
        }

    }
}
