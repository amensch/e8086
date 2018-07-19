using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class LES : LoadSegmentInstruction
    {
        public LES(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Bus.ES = Bus.GetWord(offset + 2);
        }

    }
}
