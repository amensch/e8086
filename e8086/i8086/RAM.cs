using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class RAM
    {
        private int max_memory;
        private byte[] _ram;

        public RAM(int max)
        {
            max_memory = max;
            _ram = new byte[max];
        }

        public void Load(byte[] data, int startingAddress)
        {
            if(data.GetLength(0) + startingAddress > max_memory)
            {
                throw new ArgumentOutOfRangeException("data", "Data length and startingAddress would exceed the array size");
            }

            data.CopyTo(_ram, startingAddress);
        }

        public byte this[int i]
        {
            get { return _ram[i]; }
            set { _ram[i] = value; }
        }

        public byte[] GetChunk(int startIndex, int size)
        {
            byte[] next = new byte[size];
            Array.Copy(_ram, startIndex, next, 0, size);
            return next;
        }
    }
}
