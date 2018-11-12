using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class MOV_SReg : TwoByteInstruction
    {
        public MOV_SReg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, true);
            SaveToDestination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, true);
        }

        public override long Clocks()
        {
            // seg-reg, reg or reg, seg-reg
            if (secondByte.MOD == 0x03)
            {
                return 2;
            }
            else
            {
                if (direction == 0)
                {
                    //seg-reg, mem
                    return EffectiveAddressClocks + 8;
                }
                else
                {
                    //mem, seg-reg
                    return EffectiveAddressClocks + 9;
                }
            }
        }

    }
}
