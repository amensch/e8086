using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal abstract class LogicalImmediate : LogicalInstruction
    {
        public LogicalImmediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            if (wordSize == 0)
            {
                source = secondByte.Value;
            }
            else
            {
                byte lo = secondByte.Value;
                byte hi = Bus.NextImmediate();
                source = new WordRegister(hi, lo);
            }

            // override default values
            // direction always = 0
            // mod/reg/rm set to use AL/AX and immediate
            direction = 0;
        }
    }
}
