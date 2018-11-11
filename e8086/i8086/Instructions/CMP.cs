using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CMP : SUB
    {
        public CMP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            SubWithBorrow = false;
            CompareOnly = true;
        }

        protected override void DetermineClocks()
        {
            // reg,reg
            if (secondByte.MOD == 0x03)
            {
                Clocks = 3;
            }

            // reg,mem or mem,reg
            else
            {
                Clocks = EffectiveAddressClocks + 9;
            }
        }
    }
}
