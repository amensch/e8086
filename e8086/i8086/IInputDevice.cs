using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public interface IInputDevice
    {
        int ReadData(int wordSize);
    }
}
