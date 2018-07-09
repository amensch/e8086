using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class CBW : Instruction
    {
        public CBW(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CBW - convert byte into word
        protected override void ExecuteInstruction()
        {
            if ((EU.Registers.AL & 0x80) == 0x80)
            {
                EU.Registers.AH = 0xff;
            }
            else
            {
                EU.Registers.AH = 0x00;
            }
        }

    }
}
