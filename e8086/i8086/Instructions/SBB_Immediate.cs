using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class SBB_Immediate : SUB_Immediate
    {
        public SBB_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            SubWithBorrow = true;
            CompareOnly = false;
        }
    }
}
