using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class RotateAndShiftCL : RotateAndShift
    {
        public RotateAndShiftCL(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override int GetSourceData()
        {
            return EU.Registers.CL;
        }
    }
}
