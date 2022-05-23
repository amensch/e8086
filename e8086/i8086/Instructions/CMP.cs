
namespace KDS.e8086.Instructions
{
    internal class CMP : SUB
    {
        protected override bool CompareOnly => true;

        public CMP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }

        public override long Clocks()
        {
            // reg,reg
            if (secondByte.MOD == 0x03)
            {
                return 3;
            }

            // reg,mem or mem,reg
            else
            {
                return EffectiveAddressClocks + 9;
            }
        }
    }
}
