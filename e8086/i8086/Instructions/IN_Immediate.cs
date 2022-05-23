
namespace KDS.e8086.Instructions
{
    internal class IN_Immediate : InputInstruction
    {
        public IN_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            port = Bus.NextImmediate();
        }

        public override long Clocks()
        {
            return 10;
        }
    }
}
