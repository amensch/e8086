using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class LOOPNE : LOOP
    {
        public LOOPNE(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override bool LoopConditionCheck()
        {
            return !EU.CondReg.ZeroFlag;
        }
    }
}
