using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// Op Codes: 04-05, 14-15
    /// operand1 = operand1 + operand2
    /// Flags: O S Z A P C    
    /// </summary>    
    internal class ADD_Immediate : ADD
    {
        public ADD_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            AddWithCarry = false;
        }

        protected override void ExecuteInstruction()
        { 
            int source;
            if (wordSize == 0)
            {
                source = secondByte.Value;
            }
            else
            {
                byte lo = secondByte.Value;
                byte hi = Bus.NextIP();
                source = new WordRegister(hi, lo);
            }

            // override calculated values
            // direction is always 0
            // mod/reg/rm is adjusted because these instructions are always
            // stored in AL or AX
            direction = 0;
            ADD_Destination(source, 0x03, 0x00, 0x00);
        }
    }
}
