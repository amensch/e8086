using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class AND : LogicalInstruction
    {
        public AND(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ProcessInstruction(source, secondByte.MOD, secondByte.REG, secondByte.RM, false);
        }

        protected override int Operand(int source, int dest)
        {
            return (dest & source);
        }

        public override long Clocks()
        {
            // reg,reg
            if (secondByte.MOD == 0x03)
            {
                return 3;
            }
            // mem,reg
            else if (direction == 0)
            {
                return EffectiveAddressClocks + 16;
            }
            // reg,mem
            else
            {
                return EffectiveAddressClocks + 9;
            }
        }
    }
}
