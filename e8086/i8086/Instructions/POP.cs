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
    public class POP : TwoByteInstruction
    {
        public POP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // for segment register ops, use the reg field
            if (OpCode < 0x50)
            {
                SaveWordToSegReg(OpCodeMode.REG, Pop());
            }
            // else use rm field to determine the register
            else
            {
                SaveWordToRegisters(OpCodeMode.RM, Pop());
            }
        }


    }

}
