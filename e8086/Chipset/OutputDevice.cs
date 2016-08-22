using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public delegate void WriteByteDelgate(byte data);
    public delegate void WriteWordDelegate(ushort data);

    public class OutputDevice : IOutputDevice
    {
        WriteByteDelgate _write8;
        WriteWordDelegate _write16;

        public OutputDevice(WriteByteDelgate byteDelgate, WriteWordDelegate wordDelegate)
        {
            _write8 = byteDelgate;
            _write16 = wordDelegate;
        }

        public void Write(byte data)
        {
            _write8(data);
        }

        public void Write(ushort data)
        {
            _write16(data);
        }
    }
}
