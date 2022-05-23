﻿
namespace KDS.e8086.Instructions
{
    internal class IN_Reg : InputInstruction
    {
        public IN_Reg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            port = EU.Registers.DX;
        }

        public override long Clocks()
        {
            return 8;
        }
    }
}
