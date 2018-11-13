using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class Registers
    {
        private WordRegister _ax = new WordRegister();
        private WordRegister _bx = new WordRegister();
        private WordRegister _cx = new WordRegister();
        private WordRegister _dx = new WordRegister();

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
            AX = 0;
            BX = 0;
            CX = 0;
            DX = 0;
        }

        public ushort AX
        {
            get { return _ax.Value; }
            set { _ax.Value = value; }
        }
        public ushort BX
        {
            get { return _bx.Value; }
            set { _bx.Value = value; }
        }
        public ushort CX
        {
            get { return _cx.Value; }
            set { _cx.Value = value; }
        }
        public ushort DX
        {
            get { return _dx.Value; }
            set { _dx.Value = value; }
        }
        public byte AH
        {
            get { return _ax.HI; }
            set { _ax.HI = value; }
        }
        public byte AL
        {
            get { return _ax.LO; }
            set { _ax.LO = value; }
        }
        public byte BH
        {
            get { return _bx.HI; }
            set { _bx.HI = value; }
        }
        public byte BL
        {
            get { return _bx.LO; }
            set { _bx.LO = value; }
        }
        public byte CH
        {
            get { return _cx.HI; }
            set { _cx.HI = value; }
        }
        public byte CL
        {
            get { return _cx.LO; }
            set { _cx.LO = value; }
        }
        public byte DH
        {
            get { return _dx.HI; }
            set { _dx.HI = value; }
        }
        public byte DL
        {
            get { return _dx.LO; }
            set { _dx.LO = value; }
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
                case 0x00:
                    {
                        result = AL;
                        break;
                    }
                case 0x01:
                    {
                        result = CL;
                        break;
                    }
                case 0x02:
                    {
                        result = DL;
                        break;
                    }
                case 0x03:
                    {
                        result = BL;
                        break;
                    }
                case 0x04:
                    {
                        result = AH;
                        break;
                    }
                case 0x05:
                    {
                        result = CH;
                        break;
                    }
                case 0x06:
                    {
                        result = DH;
                        break;
                    }
                case 0x07:
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
                case 0x00:
                    {
                        result = AX;
                        break;
                    }
                case 0x01:
                    {
                        result = CX;
                        break;
                    }
                case 0x02:
                    {
                        result = DX;
                        break;
                    }
                case 0x03:
                    {
                        result = BX;
                        break;
                    }
                case 0x04:
                    {
                        result = SP;
                        break;
                    }
                case 0x05:
                    {
                        result = BP;
                        break;
                    }
                case 0x06:
                    {
                        result = SI;
                        break;
                    }
                case 0x07:
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
                case 0x00:
                    {
                        AL = value;
                        break;
                    }
                case 0x01:
                    {
                        CL = value;
                        break;
                    }
                case 0x02:
                    {
                        DL = value;
                        break;
                    }
                case 0x03:
                    {
                        BL = value;
                        break;
                    }
                case 0x04:
                    {
                        AH = value;
                        break;
                    }
                case 0x05:
                    {
                        CH = value;
                        break;
                    }
                case 0x06:
                    {
                        DH = value;
                        break;
                    }
                case 0x07:
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
                case 0x00:
                    {
                        AX = value;
                        break;
                    }
                case 0x01:
                    {
                        CX = value;
                        break;
                    }
                case 0x02:
                    {
                        DX = value;
                        break;
                    }
                case 0x03:
                    {
                        BX = value;
                        break;
                    }
                case 0x04:
                    {
                        SP = value;
                        break;
                    }
                case 0x05:
                    {
                        BP = value;
                        break;
                    }
                case 0x06:
                    {
                        SI = value;
                        break;
                    }
                case 0x07:
                    {
                        DI = value;
                        break;
                    }
            }

        }
    }
}
