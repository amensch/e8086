using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CMP_ImmediateToReg : SUB_ImmediateToReg
    {
        protected override bool CompareOnly => true;

        public CMP_ImmediateToReg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }

        public override long Clocks()
        {
            if (secondByte.MOD == 0x03)
            {
                // reg,imm
                return 4;
            }
            else
            {
                // mem,imm
                return EffectiveAddressClocks + 10;
            }
        }
    }
}
