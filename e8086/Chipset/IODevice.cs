using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public interface IODevice
    {
        bool IsListening(ushort port);
        int ReadData(int wordSize, ushort port);
        void WriteData(int wordSize, ushort port, int data);
    }
}
