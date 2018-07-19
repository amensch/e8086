using System;

namespace KDS.e8086
{
    public class WordRegister
    {
        public byte HI { get; set; }
        public byte LO { get; set; }

        public WordRegister()
        {
            HI = 0;
            LO = 0;
        }

        public WordRegister(ushort data)
        {
            Value = data;
        }

        public WordRegister(byte hi, byte lo)
        {
            HI = hi;
            LO = lo;
        }

        public ushort Value
        {
            get
            {
                return (ushort)(((uint)HI << 8 | (uint)LO) & 0xffff);
            }
            set
            {
                HI = (byte)(value >> 8);
                LO = (byte)(value & 0x00ff);
            }
        }

        public override string ToString()
        {
            return Value.ToString("X4");
        }

        // Implicit type conversion
        public static implicit operator ushort(WordRegister reg)
        {
            return reg.Value;
        }

        public static implicit operator uint(WordRegister reg)
        {
            return reg.Value;
        }

    }
}
