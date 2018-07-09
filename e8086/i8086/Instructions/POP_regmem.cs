using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// Instruction 0x8f POP REG16/MEM16
    /// </summary>
    public class POP_regmem : TwoByteInstruction
    {
        public POP_regmem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            SaveToDestination(Pop(), 0, 1, secondByte.MOD, secondByte.REG, secondByte.RM);
        }
    }
}
