using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public interface IOutputDevice
    {
        int PortNumber { get; }
        void Write(byte data);
        void Write16(ushort data);
    }
}
