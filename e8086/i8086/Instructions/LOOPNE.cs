using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class LOOPNE : LOOP
    {
        public LOOPNE(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override bool LoopConditionCheck()
        {
            return !EU.CondReg.ZeroFlag;
        }

        public override long Clocks()
        {
            if (LoopConditionCheck())
            {
                return 19;
            }
            else
            {
                return 5;
            }
        }
    }
}
