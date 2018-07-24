using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class PUSHA : Instruction
    {
        public PUSHA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ushort old_sp = EU.Registers.SP;
            Push(EU.Registers.AX);
            Push(EU.Registers.CX);
            Push(EU.Registers.DX);
            Push(EU.Registers.BX);
            Push(old_sp);
            Push(EU.Registers.BP);
            Push(EU.Registers.SI);
            Push(EU.Registers.DI);
        }
    }
}
