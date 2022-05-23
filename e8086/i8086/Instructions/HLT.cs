
namespace KDS.e8086.Instructions
{
    internal class HLT : Instruction
    {
        public HLT(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.IP--;
            EU.Halted = true;
        }
    }
}
