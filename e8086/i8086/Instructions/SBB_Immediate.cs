using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class SBB_Immediate : SUB_Immediate
    {
        protected override bool SubWithBorrow => true;

        public SBB_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }
    }
}
