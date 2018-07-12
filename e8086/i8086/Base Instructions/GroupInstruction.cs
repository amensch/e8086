using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public abstract class GroupInstruction : TwoByteInstruction
    {
        public GroupInstruction(byte opCode, byte SecondByte, IExecutionUnit eu, IBus bus) : base (opCode, eu, bus)
        {
            secondByte = new AddressMode(SecondByte);
        }

        protected override void PreProcessing()
        {
            // do nothing - base class calls Bus.NextIP() which we don't want!
        }
    }
}
