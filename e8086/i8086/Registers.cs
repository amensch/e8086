using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class Registers
    {
        public const byte REGISTER_AL = 0x00;
        public const byte REGISTER_CL = 0x01;
        public const byte REGISTER_DL = 0x02;
        public const byte REGISTER_BL = 0x03;
        public const byte REGISTER_AH = 0x04;
        public const byte REGISTER_CH = 0x05;
        public const byte REGISTER_DH = 0x06;
        public const byte REGISTER_BH = 0x07;

        public const byte REGISTER_AX = 0x00;
        public const byte REGISTER_CX = 0x01;
        public const byte REGISTER_DX = 0x02;
        public const byte REGISTER_BX = 0x03;
        public const byte REGISTER_SP = 0x04;
        public const byte REGISTER_BP = 0x05;
        public const byte REGISTER_SI = 0x06;
        public const byte REGISTER_DI = 0x07;


        private WordRegister ax = new WordRegister();
        private WordRegister bx = new WordRegister();
        private WordRegister cx = new WordRegister();
        private WordRegister dx = new WordRegister();

        public ushort SP { get; set; }
        public ushort BP { get; set; }
        public ushort SI { get; set; }
        public ushort DI { get; set; }

        public Registers()
        {
            SP = 0;
            BP = 0;
            SI = 0;
            DI = 0;
        }

        public ushort AX
        {
            get { return ax.Value; }
            set { ax.Value = value; }
        }
        public ushort BX
        {
            get { return bx.Value; }
            set { bx.Value = value; }
        }
        public ushort CX
        {
            get { return cx.Value; }
            set { cx.Value = value; }
        }
        public ushort DX
        {
            get { return dx.Value; }
            set { dx.Value = value; }
        }
        public byte AH
        {
            get { return ax.HI; }
            set { ax.HI = value; }
        }
        public byte AL
        {
            get { return ax.LO; }
            set { ax.LO = value; }
        }
        public byte BH
        {
            get { return bx.HI; }
            set { bx.HI = value; }
        }
        public byte BL
        {
            get { return bx.LO; }
            set { bx.LO = value; }
        }
        public byte CH
        {
            get { return cx.HI; }
            set { cx.HI = value; }
        }
        public byte CL
        {
            get { return cx.LO; }
            set { cx.LO = value; }
        }
        public byte DH
        {
            get { return dx.HI; }
            set { dx.HI = value; }
        }
        public byte DL
        {
            get { return dx.LO; }
            set { dx.LO = value; }
        }

        public int GetRegisterValue(int wordSize, byte reg)
        {
            if(wordSize == 0)
            {
                return GetByteFromRegisters(reg);
            }
            else
            {
                return GetWordFromRegisters(reg);
            }
        }

        public void SaveRegisterValue(int wordSize, byte reg, int value)
        {
            if (wordSize == 0)
            {
                SaveByteToRegisters(reg, (byte)(value & 0xff));
            }
            else
            {
                SaveWordToRegisters(reg, (ushort)(value & 0xffff));
            }
        }

        // Get 8 bit REG result (or R/M mod=11)
        private byte GetByteFromRegisters(byte reg)
        {
            byte result = 0;
            switch (reg)
            {
                case REGISTER_AL:
                    {
                        result = AL;
                        break;
                    }
                case REGISTER_CL:
                    {
                        result = CL;
                        break;
                    }
                case REGISTER_DL:
                    {
                        result = DL;
                        break;
                    }
                case REGISTER_BL:
                    {
                        result = BL;
                        break;
                    }
                case REGISTER_AH:
                    {
                        result = AH;
                        break;
                    }
                case REGISTER_CH:
                    {
                        result = CH;
                        break;
                    }
                case REGISTER_DH:
                    {
                        result = DH;
                        break;
                    }
                case REGISTER_BH:
                    {
                        result = BH;
                        break;
                    }
            }
            return result;
        }

        // Get 16 bit REG result (or R/M mod=11)
        private ushort GetWordFromRegisters(byte reg)
        {
            ushort result = 0;
            switch (reg)
            {
                case REGISTER_AX:
                    {
                        result = AX;
                        break;
                    }
                case REGISTER_CX:
                    {
                        result = CX;
                        break;
                    }
                case REGISTER_DX:
                    {
                        result = DX;
                        break;
                    }
                case REGISTER_BX:
                    {
                        result = BX;
                        break;
                    }
                case REGISTER_SP:
                    {
                        result = SP;
                        break;
                    }
                case REGISTER_BP:
                    {
                        result = BP;
                        break;
                    }
                case REGISTER_SI:
                    {
                        result = SI;
                        break;
                    }
                case REGISTER_DI:
                    {
                        result = DI;
                        break;
                    }
            }
            return result;
        }

        // Save 8 bit value in register indicated by REG
        private void SaveByteToRegisters(byte reg, byte value)
        {
            switch (reg)
            {
                case REGISTER_AL:
                    {
                        AL = value;
                        break;
                    }
                case REGISTER_CL:
                    {
                        CL = value;
                        break;
                    }
                case REGISTER_DL:
                    {
                        DL = value;
                        break;
                    }
                case REGISTER_BL:
                    {
                        BL = value;
                        break;
                    }
                case REGISTER_AH:
                    {
                        AH = value;
                        break;
                    }
                case REGISTER_CH:
                    {
                        CH = value;
                        break;
                    }
                case REGISTER_DH:
                    {
                        DH = value;
                        break;
                    }
                case REGISTER_BH:
                    {
                        BH = value;
                        break;
                    }
            }

        }

        // Save 16 bit value in register indicated by REG
        private void SaveWordToRegisters(byte reg, ushort value)
        {
            switch (reg)
            {
                case REGISTER_AX:
                    {
                        AX = value;
                        break;
                    }
                case REGISTER_CX:
                    {
                        CX = value;
                        break;
                    }
                case REGISTER_DX:
                    {
                        DX = value;
                        break;
                    }
                case REGISTER_BX:
                    {
                        BX = value;
                        break;
                    }
                case REGISTER_SP:
                    {
                        SP = value;
                        break;
                    }
                case REGISTER_BP:
                    {
                        BP = value;
                        break;
                    }
                case REGISTER_SI:
                    {
                        SI = value;
                        break;
                    }
                case REGISTER_DI:
                    {
                        DI = value;
                        break;
                    }
            }
        }
    }
}
