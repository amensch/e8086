
namespace KDS.e8086.Instructions
{
    internal class CMP : SUB
    {
        protected override bool CompareOnly => true;

        public CMP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }
    }
}
