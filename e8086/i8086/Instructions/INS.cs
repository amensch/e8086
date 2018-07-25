using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class INS : RepeatableInstruction
    {
        public INS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            if(wordSize == 0)
            {
                Bus.SaveByteString(EU.Registers.DI, (byte)EU.ReadPort(wordSize, EU.Registers.DX));
            }
            else
            {
                Bus.SaveWordString(EU.Registers.DI, (ushort)EU.ReadPort(wordSize, EU.Registers.DX));
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
