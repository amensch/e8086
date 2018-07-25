using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class ADC_Immediate : ADD_Immediate
    {
        public ADC_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            AddWithCarry = true;
        }
    }
}
