using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086ConditionalRegister
    {

        public enum Flags : UInt16
        {
            CARRY_FLAG = 0x0001,       // 0=no carry, 1=carry
            PARITY_FLAG = 0x0004,      // 0=odd, 1=even
            AUX_CARRY_FLAG = 0x0010,
            ZERO_FLAG = 0x0040,        // 0=non-zero, 1=zero
            SIGN_FLAG = 0x0080,        // 0=positive, 1=negative
            TRAP_FLAG = 0x0100,
            INT_ENABLE = 0x0200,
            DIR_FLAG = 0x0400,
            OVERFLOW_FLAG = 0x800
        };
        
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
                return GetBit(Flags.CARRY_FLAG);
            }
            set
            {
                SetBit(Flags.CARRY_FLAG, value);
            }
        }
        public bool SignFlag
        {
            get
            {
                return GetBit(Flags.SIGN_FLAG);
            }
            set
            {
                SetBit(Flags.SIGN_FLAG, value);
            }
        }
        public bool ZeroFlag
        {
            get
            {
                return GetBit(Flags.ZERO_FLAG);
            }
            set
            {
                SetBit(Flags.ZERO_FLAG, value);
            }
        }
        public bool AuxCarryFlag
        {
            get
            {
                return GetBit(Flags.AUX_CARRY_FLAG);
            }
            set
            {
                SetBit(Flags.AUX_CARRY_FLAG, value);
            }
        }
        public bool ParityFlag
        {
            get
            {
                return GetBit(Flags.PARITY_FLAG);
            }
            set
            {
                SetBit(Flags.PARITY_FLAG, value);
            }
        }
        public bool TrapFlag
        {
            get
            {
                return GetBit(Flags.TRAP_FLAG);
            }
            set
            {
                SetBit(Flags.TRAP_FLAG, value);
            }
        }
        public bool InterruptEnable
        {
            get
            {
                return GetBit(Flags.INT_ENABLE);
            }
            set
            {
                SetBit(Flags.INT_ENABLE, value);
            }
        }
        public bool DirectionFlag
        {
            get
            {
                return GetBit(Flags.DIR_FLAG);
            }
            set
            {
                SetBit(Flags.DIR_FLAG, value);
            }
        }
        public bool OverflowFlag
        {
            get
            {
                return GetBit(Flags.OVERFLOW_FLAG);
            }
            set
            {
                SetBit(Flags.OVERFLOW_FLAG, value);
            }
        }


        public void CalcCarryFlag(int word_size, int result)
        {
            if (word_size == 0)
                CarryFlag = ((UInt16)result > 0xff);
            else
            {
                CarryFlag = ((UInt32)result > 0xffff);
            }
        }

        public void CalcOverflowFlag( int word_size, int src, int dest)
        {
            int result = src + dest;
            if (word_size == 0)
                OverflowFlag = ((result ^ src) & (result ^ dest) & 0x80) == 0x80;
            else
                OverflowFlag = ((result ^ src) & (result ^ dest) & 0x8000) == 0x8000;
        }

        public void CalcOverflowSubtract( int word_size, int src, int dest)
        {
            int result = dest - src;
            if (word_size == 0)
                OverflowFlag = ((result ^ dest) & (src ^ dest) & 0x80) == 0x80;
            else
                OverflowFlag = ((result ^ dest) & (src ^ dest) & 0x8000) == 0x8000;
        }

        public void CalcAuxCarryFlag( int src, int dst )
        {
            int result = src + dst;
            AuxCarryFlag = ((src ^ dst ^ result) & 0x10) == 0x10;
        }

        public void CalcZeroFlag(int word_size, int result)
        {
            if (word_size == 0)
                ZeroFlag = ((result & 0xff) == 0);
            else
                ZeroFlag = ((result & 0xffff) == 0);
        }

        public void CalcSignFlag(int word_size, int result)
        {
            if( word_size == 0)
                SignFlag = ((result & 0x80) == 0x80);
            else
                SignFlag = ((result & 0x8000) == 0x8000);
        }

        public void CalcParityFlag(int result)
        {
            // parity = 0 is odd
            // parity = 1 is even
            // Always uses lower 8 bits of the result even in 16 bit operations
            byte num = (byte)(result & 0xff);
            byte total = 0;
            for (total = 0; num > 0; total++)
            {
                num &= (byte)(num - 1);
            }
            ParityFlag = (total % 2) == 0;
        }

        private bool GetBit(Flags bit)
        {
            return ((Register & (UInt16)bit) == (UInt16)bit);
        }

        private void SetBit(Flags bit, bool set)
        {
            if (set)
                Register = (UInt16)(Register | (UInt16)bit);
            else
                Register = (UInt16)(Register & ~(UInt16)bit);
        }
    }
}
