using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class INS : RepeatableInstruction
    {
        public INS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            IODevice device;
            if(EU.TryGetDevice(EU.Registers.DX, out device))
            {
                if(wordSize == 0)
                    Bus.SaveByteString(EU.Registers.DI, (byte)device.ReadData(wordSize));
                else
                    Bus.SaveWordString(EU.Registers.DI, (ushort)device.ReadData(wordSize));
            }
            else
            {
                // zero out the register if no port attached
                if (wordSize == 0)
                    Bus.SaveByteString(EU.Registers.DI, 0);
                else
                    Bus.SaveWordString(EU.Registers.DI, 0);
            }

            if (EU.CondReg.DirectionFlag)
            {
                if (wordSize == 0)
                    EU.Registers.DI--;
                else
                    EU.Registers.DI -= 2;
            }
            else
            {
                if (wordSize == 0)
                    EU.Registers.DI++;
                else
                    EU.Registers.DI += 2;
            }
        }
    }
}
