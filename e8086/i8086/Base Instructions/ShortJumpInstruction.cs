
namespace KDS.e8086.Instructions
{
    internal abstract class ShortJumpInstruction : Instruction
    {
        public ShortJumpInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // JMP does not save anything to the stack and no return is expected.
        // Intrasegment JMP changes IP by adding the relative displacement from the instruction.

        // JMP IP-INC8  8 bit signed increment to the instruction pointer

        protected abstract bool JumpDecision();

        protected override void ExecuteInstruction()
        {
            if (JumpDecision())
            {
                ushort oper = SignExtendByteToWord(Bus.NextImmediate());
                Bus.IP += oper;
            }
            else
            {
                Bus.NextImmediate();  // skip over the immediate jump value
            }
        }
    }
}
