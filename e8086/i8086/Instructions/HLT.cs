
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
<<<<<<< HEAD
=======

        public override long Clocks()
        {
            return 2;
        }

>>>>>>> 0184143f5e9e3e29b837b05293c2c1fa56ba2b9c
    }
}
