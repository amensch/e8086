using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class OUT_Reg : OutputInstruction
    {
        public OUT_Reg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            port = EU.Registers.DX;
        }
    }
}
