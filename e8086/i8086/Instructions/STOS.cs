using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class STOS : RepeatableInstruction
    {
        public STOS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            if (wordSize == 0)
            {
                Bus.SaveByteString(EU.Registers.DI, EU.Registers.AL);
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
                Bus.SaveWordString(EU.Registers.DI, EU.Registers.AX);
                if (EU.CondReg.DirectionFlag)
                {
                    EU.Registers.DI -= 2;
                }
                else
                {
                    EU.Registers.DI += 2;
                }
            }
        }

    }
}
