using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class LODS : RepeatableInstruction
    {
        public LODS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            if (wordSize == 0)
            {
                EU.Registers.AL = Bus.GetByte(EU.Registers.SI);
                if (EU.CondReg.DirectionFlag)
                {
                    EU.Registers.SI--;
                }
                else
                {
                    EU.Registers.SI++;
                }
            }
            else
            {
                EU.Registers.AX = Bus.GetWord(EU.Registers.SI);
                if (EU.CondReg.DirectionFlag)
                {
                    EU.Registers.SI -= 2;
                }
                else
                {
                    EU.Registers.SI += 2;
                }
            }
        }

    }
}
