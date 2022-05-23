﻿
namespace KDS.e8086.Instructions
{
    internal class ADC_Immediate : ADD_Immediate
    {
        protected override bool AddWithCarry => true;
        public ADC_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }
    }
}
