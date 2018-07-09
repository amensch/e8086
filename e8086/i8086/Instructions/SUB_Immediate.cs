using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// Op Codes: SUB 2C-2D, SBB 1C-1D, CMP 3C-3D
    ///         : CMP 3C-3D
    /// operand1 = operand1 - operand2
    /// Flags: O S Z A P C
    /// </summary>
    public class SUB_Immediate : SUB
    {
        public SUB_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {

        }


        protected override void ExecuteInstruction()
        {
            int source;  // source is the next byte or word
            if (wordSize == 0)
            {
                source = secondByte.Value;
            }
            else
            {
                byte lo = secondByte.Value;
                byte hi = Bus.NextIP();
                source = new DataRegister16(hi, lo);
            }

            // override calculated values
            // direction is always 0
            // mod/reg/rm is adjusted because these instructions are always
            // stored in AL or AX

            direction = 0;
            SUB_Destination(source, 0x03, 0x00, 0x00);
        }
    }
}
