using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086ConditionalRegister
    {
        private const UInt16 OVERFLOW_FLAG = 0x800;
        private const UInt16 DIR_FLAG = 0x0400;
        private const UInt16 INT_ENABLE = 0x0200;
        private const UInt16 TRAP_FLAG = 0x0100;
        private const UInt16 SIGN_FLAG = 0x0080;        // 0=positive, 1=negative
        private const UInt16 ZERO_FLAG = 0x0040;        // 0=non-zero, 1=zero
        private const UInt16 AUX_CARRY_FLAG = 0x0010;   
        private const UInt16 PARITY_FLAG = 0x0004;      // 0=odd, 1=even
        private const UInt16 CARRY_FLAG = 0x0001;       // 0=no carry, 1=carry

        public UInt16 Register
        {
            get;
            set;
        }

        public i8086ConditionalRegister()
        {
            Register = 0x0000;
        }

        public bool CarryFlag
        {
            get
            {
                return GetBit(CARRY_FLAG);
            }
            set
            {
                SetBit(CARRY_FLAG, value);
            }
        }
        public bool SignFlag
        {
            get
            {
                return GetBit(SIGN_FLAG);
            }
            set
            {
                SetBit(SIGN_FLAG, value);
            }
        }
        public bool ZeroFlag
        {
            get
            {
                return GetBit(ZERO_FLAG);
            }
            set
            {
                SetBit(ZERO_FLAG, value);
            }
        }
        public bool AuxCarryFlag
        {
            get
            {
                return GetBit(AUX_CARRY_FLAG);
            }
            set
            {
                SetBit(AUX_CARRY_FLAG, value);
            }
        }
        public bool ParityFlag
        {
            get
            {
                return GetBit(PARITY_FLAG);
            }
            set
            {
                SetBit(PARITY_FLAG, value);
            }
        }
        public bool TrapFlag
        {
            get
            {
                return GetBit(TRAP_FLAG);
            }
            set
            {
                SetBit(TRAP_FLAG, value);
            }
        }
        public bool InterruptEnable
        {
            get
            {
                return GetBit(INT_ENABLE);
            }
            set
            {
                SetBit(INT_ENABLE, value);
            }
        }
        public bool DirectionFlag
        {
            get
            {
                return GetBit(DIR_FLAG);
            }
            set
            {
                SetBit(DIR_FLAG, value);
            }
        }
        public bool OverflowFlag
        {
            get
            {
                return GetBit(OVERFLOW_FLAG);
            }
            set
            {
                SetBit(OVERFLOW_FLAG, value);
            }
        }


        public void CalcCarryFlag(UInt16 result)
        {
            CarryFlag = (result > 0xff);
        }

        public void CalcZeroFlag(UInt16 result)
        {
            ZeroFlag = ((result & 0xff) == 0);
        }

        public void CalcSignFlag(UInt16 result)
        {
            SignFlag = ((result & 0x80) == 0x80);
        }

        public void CalcParityFlag(UInt16 result)
        {
            // parity = 0 is odd
            // parity = 1 is even
            CalcParityFlag((byte)(result & 0xff));
        }

        public void CalcParityFlag(byte result)
        {
            // parity = 0 is odd
            // parity = 1 is even
            byte num = (byte)(result & 0xff);
            byte total = 0;
            for (total = 0; num > 0; total++)
            {
                num &= (byte)(num - 1);
            }
            ParityFlag = (total % 2) == 0;
        }

        public void CalcAuxCarryFlag(byte a, byte b)
        {
            AuxCarryFlag = (byte)((a & 0x0f) + (b & 0x0f)) > 0x0f;
        }

        public void CalcAuxCarryFlag(byte a, byte b, byte c)
        {
            AuxCarryFlag = (byte)((a & 0x0f) + (b & 0x0f) + (c & 0x0f)) > 0x0f;
        }

        private bool GetBit(UInt16 bit)
        {
            return ((Register & bit) == bit);
        }

        private void SetBit(UInt16 bit, bool set)
        {
            if (set)
                Register = (UInt16)(Register | bit);
            else
                Register = (UInt16)(Register & ~bit);
        }
    }
}
