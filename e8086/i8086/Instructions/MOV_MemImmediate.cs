using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// MOV MEM <-> IMM (8 and 16)
    /// </summary>
    public class MOV_MemImmediate : Instruction
    {
        public MOV_MemImmediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            byte lo = Bus.NextIP();
            byte hi = Bus.NextIP();
            switch (OpCode)
            {
                case 0xa0:
                    {
                        EU.Registers.AL = Bus.GetData8(new DataRegister16(hi, lo));
                        break;
                    }
                case 0xa1:
                    {
                        EU.Registers.AX = Bus.GetData16(new DataRegister16(hi, lo));
                        break;
                    }
                case 0xa2:
                    {
                        Bus.SaveData8(new DataRegister16(hi, lo), EU.Registers.AL);
                        break;
                    }
                case 0xa3:
                    {
                        Bus.SaveData16(new DataRegister16(hi, lo), EU.Registers.AX);
                        break;
                    }
            }

        }
    }
}
