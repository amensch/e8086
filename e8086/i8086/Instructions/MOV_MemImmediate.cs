using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// MOV MEM <-> IMM (8 and 16)
    /// </summary>
    internal class MOV_MemImmediate : Instruction
    {
        public MOV_MemImmediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            byte lo = Bus.NextImmediate();
            byte hi = Bus.NextImmediate();
            switch (OpCode)
            {
                case 0xa0:
                    {
                        EU.Registers.AL = (byte)(Bus.GetData(0, new WordRegister(hi, lo)) & 0xff);
                        break;
                    }
                case 0xa1:
                    {
                        EU.Registers.AX = (ushort)(Bus.GetData(1, new WordRegister(hi, lo)) & 0xffff);
                        break;
                    }
                case 0xa2:
                    {
                        Bus.SaveData(0, new WordRegister(hi, lo), EU.Registers.AL);
                        break;
                    }
                case 0xa3:
                    {
                        Bus.SaveData(1, new WordRegister(hi, lo), EU.Registers.AX);
                        break;
                    }
            }

        }

        protected override void DetermineClocks()
        {
            //mem,acc and acc,mem
            Clocks = 10 ;
        }
    }
}
