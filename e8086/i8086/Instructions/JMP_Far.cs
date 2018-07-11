using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class JMP_Far : Instruction
    {
        public JMP_Far(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort nextIP = GetImmediate16();
            ushort nextCS = GetImmediate16();
            Bus.IP = nextIP;
            Bus.CS = nextCS;
        }
    }
}
