using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086CPU
    {
        private i8086ExecutionUnit _eu;

        public i8086CPU()
        {
        }

        public void Boot(byte[] program)
        {
            // For now we will hard code the BIOS to start at a particular code segment.
            _eu = new i8086ExecutionUnit(new i8086BusInterfaceUnit(0x0000, 0x0000, program));

        }

    }
}
