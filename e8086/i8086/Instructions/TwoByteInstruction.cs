using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public abstract class TwoByteInstruction : Instruction
    {
        protected AddressMode secondByte;



        public TwoByteInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            secondByte = new AddressMode(Bus.NextIP());
        }


 
    }
}
