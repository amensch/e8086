
namespace KDS.e8086
{
    internal class AddressMode
    {
        private byte value;

        public byte MOD { get; private set; }
        public byte REG { get; private set; }
        public byte RM { get; private set; }

        public AddressMode(byte addr)
        {
            Value = addr;
        }

        public byte Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                SplitAddrByte(value);
            }
        }

        private void SplitAddrByte(byte addr)
        {
            MOD = (byte)((addr >> 6) & 0x03);
            REG = (byte)((addr >> 3) & 0x07);
            RM = (byte)(addr & 0x07);
        }
    }
}
