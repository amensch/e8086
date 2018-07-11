using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// OpCodes b0-b7: MOV <reg>, IMM-8
    /// </summary>
    public class MOV_ImmediateByte : Instruction
    {
        public MOV_ImmediateByte(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // use op code like the reg field to define the register
            byte reg = (byte)(OpCode & 0x07);
            byte value = Bus.NextIP();
            SaveByteToRegisters(reg, value);
        }

    }
}
