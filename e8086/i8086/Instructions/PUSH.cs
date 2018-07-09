using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// 
    /// </summary>
    public class PUSH : Instruction
    {
        public PUSH(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // for segment register ops, use the reg field
            if (OpCode < 0x50)
            {
                Push(GetSegRegField(OpCodeMode.REG));
            }
            // else use rm field to determine the register
            else
            {
                Push(GetRegField16(OpCodeMode.RM));
            }
        }

    }

}
