using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class OUT_Immediate : OutputInstruction
    {
        public OUT_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            port = Bus.NextImmediate();
        }
    }
}
