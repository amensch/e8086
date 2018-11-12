using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// AAA: Ascii Adjust Add
    /// </summary>
    internal class AAA : Instruction
    {
        public AAA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            if ((EU.Registers.AL > 9) | EU.CondReg.AuxCarryFlag)
            {
                EU.Registers.AL += 6;
                EU.Registers.AH += 1;
                EU.CondReg.AuxCarryFlag = true;
                EU.CondReg.CarryFlag = true;
            }
            else
            {
                EU.CondReg.AuxCarryFlag = false;
                EU.CondReg.CarryFlag = false;
            }

            // clear high nibble of AL
            EU.Registers.AL = (byte)(EU.Registers.AL & 0x0f);
        }

        public override long Clocks()
        {
            return 4;
        }
    }
}
