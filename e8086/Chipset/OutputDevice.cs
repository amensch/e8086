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
        WriteByteDelgate writeByte;
        WriteWordDelegate writeWord;

        public OutputDevice(WriteByteDelgate byteDelgate, WriteWordDelegate wordDelegate)
        {
            writeByte = byteDelgate;
            writeWord = wordDelegate;
        }

        public void Write(byte data)
        {
            writeByte(data);
        }

        public void Write(ushort data)
        {
            writeWord(data);
        }
    }
}
