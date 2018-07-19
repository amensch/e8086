using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public delegate byte ReadByteDelegate();
    public delegate ushort ReadWordDelegate();

    public class InputDevice : IInputDevice
    {
        ReadByteDelegate readByteDelegate;
        ReadWordDelegate readWordDelegate;

        public InputDevice(ReadByteDelegate byteDelgate, ReadWordDelegate wordDelegate)
        {
            readByteDelegate = byteDelgate;
            readWordDelegate = wordDelegate;
        }

        public byte ReadByte()
        {
            return readByteDelegate();
        }

        public ushort ReadWord()
        {
            return readWordDelegate();
        }
    }
}
