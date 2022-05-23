
namespace KDS.e8086.Instructions
{
    internal class SBB : SUB
    {
        protected override bool SubWithBorrow => true;

        public SBB(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }
    }
}
