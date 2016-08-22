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
        ReadByteDelegate _read8;
        ReadWordDelegate _read16;

        public InputDevice(ReadByteDelegate byteDelgate, ReadWordDelegate wordDelegate)
        {
            _read8 = byteDelgate;
            _read16 = wordDelegate;
        }

        public byte Read()
        {
            return _read8();
        }

        public ushort Read16()
        {
            return _read16();
        }
    }
}
