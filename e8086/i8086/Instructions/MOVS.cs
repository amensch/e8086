using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class MOVS : RepeatableInstruction
    {
        public MOVS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void DoInstruction()
        {
            Bus.MoveString(wordSize, EU.Registers.SI, EU.Registers.DI);
            if(EU.CondReg.DirectionFlag)
            {
                EU.Registers.SI -= (ushort)(wordSize + 1);
                EU.Registers.DI -= (ushort)(wordSize + 1);
            }
            else
            {
                EU.Registers.SI += (ushort)(wordSize + 1);
                EU.Registers.DI += (ushort)(wordSize + 1);
            }
        }

    }
}
