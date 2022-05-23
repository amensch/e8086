using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class LES : LoadSegmentInstruction
    {
        public LES(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.ES = (ushort)(Bus.GetData(1, offset + 2) & 0xffff);
        }

        public override long Clocks()
        {
            return EffectiveAddressClocks + 16;
        }
    }
}
