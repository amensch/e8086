using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class OUTS : RepeatableInstruction
    {
        public OUTS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            EU.WritePort(wordSize, EU.Registers.DX, Bus.GetDestString(wordSize, EU.Registers.SI));

            if (EU.CondReg.DirectionFlag)
            {
                if (wordSize == 0)
                    EU.Registers.SI--;
                else
                    EU.Registers.SI -= 2;
            }
            else
            {
                if (wordSize == 0)
                    EU.Registers.SI++;
                else
                    EU.Registers.SI += 2;
            }
        }
    }
}
