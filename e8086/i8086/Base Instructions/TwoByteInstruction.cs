
namespace KDS.e8086.Instructions
{
    internal abstract class TwoByteInstruction : Instruction
    {
        private bool FetchSecondByte = true;
        protected AddressMode secondByte;

        public TwoByteInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            if (FetchSecondByte)
            {
                secondByte = new AddressMode(Bus.NextImmediate());
            }
        }

        public void SetSecondByte(byte second)
        {
            FetchSecondByte = false;
            secondByte = new AddressMode(second);
        }
    }
}
