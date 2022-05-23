
namespace KDS.e8086.Instructions
{
    internal class CBW : Instruction
    {
        public CBW(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // CBW - convert byte into word
        protected override void ExecuteInstruction()
        {
            if ((EU.Registers.AL & 0x80) == 0x80)
            {
                EU.Registers.AH = 0xff;
            }
            else
            {
                EU.Registers.AH = 0x00;
            }
        }

    }
}
