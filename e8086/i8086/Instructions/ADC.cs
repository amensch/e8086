using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class ADC : ADD
    {
        public ADC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            AddWithCarry = true;
        }
    }
}
