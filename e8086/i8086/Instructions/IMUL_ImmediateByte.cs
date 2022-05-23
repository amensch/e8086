
namespace KDS.e8086.Instructions
{
    internal class IMUL_ImmediateByte : MultiplyInstruction
    {
        public IMUL_ImmediateByte(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override ushort GetOperand()
        {
            return Bus.NextImmediate();
        }
    }
}
