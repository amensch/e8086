using System;

namespace KDS.e8086
{
    public class DataRegister16
    {
        public byte HI { get; set; }
        public byte LO { get; set; }

        public DataRegister16()
        {
            HI = 0;
            LO = 0;
        }

        public DataRegister16(ushort _data)
        {
            Register = _data;
        }

        public DataRegister16(byte _hi, byte _lo)
        {
            HI = _hi;
            LO = _lo;
        }

        public ushort Register
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
            return Register.ToString("X4");
        }

        // Implicit type conversion
        public static implicit operator ushort(DataRegister16 reg)
        {
            return reg.Register;
        }

        public static implicit operator uint(DataRegister16 reg)
        {
            return reg.Register;
        }

    }
}
