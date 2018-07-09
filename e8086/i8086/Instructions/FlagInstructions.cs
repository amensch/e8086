using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// Catch all class for one byte flag operation instructions because lazy.
    /// </summary>
    public class FlagInstructions : Instruction
    {
        public FlagInstructions(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // CMC - complement carry flag
            if (OpCode == 0xf5)
            {
                EU.CondReg.CarryFlag = !EU.CondReg.CarryFlag;
            }
            // CLC - clear carry flag
            else if(OpCode == 0xf8)
            {
                EU.CondReg.CarryFlag = false;
            }
            // STC - set carry flag
            else if(OpCode == 0xf9)
            {
                EU.CondReg.CarryFlag = true;
            }
            // CLI - clear interrupt flag
            else if(OpCode == 0xfa)
            {
                EU.CondReg.InterruptEnable = false;
            }
            // STI - set interrupt flag
            else if(OpCode == 0xfb)
            {
                EU.CondReg.InterruptEnable = true;
            }
            // CLD - clear direction flag
            else if(OpCode == 0xfc)
            {
                EU.CondReg.DirectionFlag = false;
            }
            // STD - set direction flag
            else if(OpCode == 0xfd)
            {
                EU.CondReg.DirectionFlag = true;
            }
        }
    }
}
