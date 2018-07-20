using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public delegate int ReadDeviceDelegate(int wordSize);
    public delegate void WriteDataDelegate(int wordSize, int data);

    public class IODevice : IInputDevice, IOutputDevice
    {
        protected ReadDeviceDelegate readDelegate;
        protected WriteDataDelegate writeData;

        public IODevice(ReadDeviceDelegate read, WriteDataDelegate write)
        {
            readDelegate = read;
            writeData = write;
        }

        public int ReadData(int wordSize)
        {
            return readDelegate(wordSize);
        }

        public void Write(int wordSize, int data)
        {
            writeData(wordSize, data);
        }
    }
}
