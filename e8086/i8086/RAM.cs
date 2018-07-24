using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class RAM
    {
        private int MaxMemory;
        private byte[] ram;

        public RAM(int max)
        {
            MaxMemory = max;
            ram = new byte[max];
        }

        public void Load(byte[] data, int startingAddress)
        {
            if(data.GetLength(0) + startingAddress > MaxMemory)
            {
                throw new ArgumentOutOfRangeException("data", "Data length and startingAddress would exceed the array size");
            }

            data.CopyTo(ram, startingAddress);
        }

        public byte this[int i]
        {
            get { return ram[i]; }
            set { ram[i] = value; }
        }

        public byte[] GetChunk(int startIndex, int size)
        {
            byte[] next = new byte[size];
            Array.Copy(ram, startIndex, next, 0, size);
            return next;
        }
    }
}
