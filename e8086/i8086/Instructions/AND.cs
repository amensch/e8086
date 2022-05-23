
namespace KDS.e8086.Instructions
{
    internal class AND : LogicalInstruction
    {
        public AND(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ProcessInstruction(source, secondByte.MOD, secondByte.REG, secondByte.RM, false);
        }

        protected override int Operand(int source, int dest)
        {
            return (dest & source);
        }
    }
}
