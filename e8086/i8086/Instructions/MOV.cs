using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class MOV : TwoByteInstruction
    {
        public MOV(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            SaveToDestination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        protected override void DetermineClocks()
        {
            if (secondByte.MOD == 0x03)
            {
                // reg, reg
                Clocks = 2;
            }
            else
            {
                if (direction == 0)
                {
                    // reg, mem
                    Clocks = EffectiveAddressClocks + 8;
                }
                else
                {
                    // mem, reg
                    Clocks = EffectiveAddressClocks + 9;
                }
            }
        }
    }
}
