﻿
namespace KDS.e8086.Instructions
{
    internal class AND_Immediate : LogicalImmediate
    {
        public AND_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ProcessInstruction(source, 0x03, 0x00, 0x00, false);
        }

        protected override int Operand(int source, int dest)
        {
            return (dest & source);
        }
    }
}
