
namespace KDS.e8086.Instructions
{
    internal class ADC : ADD
    {
        protected override bool AddWithCarry => true;
        public ADC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }
    }
}
