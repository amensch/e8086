using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class OUTS : RepeatableInstruction
    {
        public OUTS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            IOutputDevice device;
            if (EU.TryGetOutputDevice(EU.Registers.DX, out device))
            {
                if(wordSize == 0)
                    device.Write(Bus.GetByteDestString(EU.Registers.SI));
                else
                    device.Write(Bus.GetWordDestString(EU.Registers.SI));
            }


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
