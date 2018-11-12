using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CALL_Far : Instruction
    {
        public CALL_Far(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort nextIP = GetImmediateWord();
            ushort nextCS = GetImmediateWord();
            Push(Bus.CS);
            Push(Bus.IP);
            Bus.IP = nextIP;
            Bus.CS = nextCS;
        }

        public override long Clocks()
        {
            return 28;
        }
    }
}
