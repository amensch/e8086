using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class SBB : SUB
    {
        public SBB(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            SubWithBorrow = true;
            CompareOnly = false;
        }
    }
}
