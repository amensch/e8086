using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{

    /*
        This class represents the Intel 8259 chip for x86 systems.  On original PCs this was a separate chip.
        Eventually equivalent circuitry is sill included on modern x86 PCs.

        The 8259 is a Programmable Interrupt Controller (PIC).  This chip combines multiple interrupt sources
        into a single interrupt output to the host processor which extends the interrupt levels available beyond
        what the CPU allows.  
    */

    public class i8259 : IInputDevice, IOutputDevice
    {
        public byte Read()
        {
            throw new NotImplementedException();
        }

        public ushort Read16()
        {
            throw new NotImplementedException();
        }

        public void Write(byte data)
        {
            throw new NotImplementedException();
        }

        public void Write16(ushort data)
        {
            throw new NotImplementedException();
        }
    }
}
