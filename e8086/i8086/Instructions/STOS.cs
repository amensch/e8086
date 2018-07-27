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
            Bus.SaveString(wordSize, EU.Registers.DI, EU.Registers.AX);

            if(EU.CondReg.DirectionFlag)
            {
                EU.Registers.DI -= (ushort)(wordSize + 1);
            }
            else
            {
                EU.Registers.DI += (ushort)(wordSize + 1);
            }
        }

    }
}
