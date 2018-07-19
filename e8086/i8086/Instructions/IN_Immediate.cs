using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class IN_Immediate : InputInstruction
    {
        public IN_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            port = Bus.NextIP();
        }
    }
}
