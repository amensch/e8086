using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class XCHG : TwoByteInstruction
    {
        public XCHG(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // Op Codes: 86-87
        // no flags
        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int dest = GetDestinationData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);

            SaveToDestination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);

            // flip destination so this is saved to the original source
            direction ^= 1;

            SaveToDestination(dest, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);

        }

        public override long Clocks()
        {
            if(secondByte.MOD == 0x03)
            {
                return 4;
            }
            else
            {
                return EffectiveAddressClocks + 17;
            }
        }
    }
}
