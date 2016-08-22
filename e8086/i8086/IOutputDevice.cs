using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public interface IOutputDevice
    {
        void Write(byte data);
        void Write(ushort data);
    }
}
