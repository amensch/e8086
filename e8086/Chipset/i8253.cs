using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /*
        This class represents the Intel 8253 chip for x86 systems.  On original PCs this was a separate chip.
        Eventually equivalent circuitry is sill included on modern x86 PCs.

        The 8253 is a Programmable Interval Timer (PIT).
    */

    public class i8253 : IInputDevice, IOutputDevice
    {
        private int _port;

        public i8253(int port)
        {
            _port = port;
        }

        public int PortNumber
        {
            get
            {
                return _port;
            }
        }

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
