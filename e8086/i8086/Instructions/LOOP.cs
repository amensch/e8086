using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class LOOP : Instruction
    {
        public LOOP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected virtual bool LoopConditionCheck()
        {
            return true;
        }

        protected override void ExecuteInstruction()
        {
            ushort oper = SignExtendByteToWord(Bus.NextImmediate());
            EU.Registers.CX--;
            if(EU.Registers.CX != 0)
            {
                if(LoopConditionCheck())
                {
                    Bus.IP += (byte)(oper & 0xffff);
                }
            }
        }

        public override long Clocks()
        {
            if(LoopConditionCheck())
            {
                return 17;
            }
            else
            {
                return 5;
            }
        }
    }
}
