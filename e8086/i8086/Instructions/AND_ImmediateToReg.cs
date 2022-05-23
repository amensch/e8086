using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class AND_ImmediateToReg : AND
    {
        public AND_ImmediateToReg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();

            // If displacement offset is needed, those bytes will appear before the immediate data so we need to retrieve that first.
            // The offset isn't needed within here but we need to retrieve it first.
            if (secondByte.MOD == 0x00)
            {
                _ = GetRMTable1(secondByte.RM);
            }
            else if ((secondByte.MOD == 0x01) || (secondByte.MOD == 0x02))
            {
                _ = GetRMTable2(secondByte.MOD, secondByte.RM);
            }
        }

        protected override void ExecuteInstruction()
        {
            // Get source data
            int source = 0;

            if (wordSize == 0)
            {
                source = Bus.NextImmediate();
            }
            else
            {
                source = GetImmediateWord();
            }

            // Direction is always 0
            direction = 0;

            ExecuteInstruction(source, secondByte.MOD, secondByte.REG, secondByte.RM, false);
        }
    }
}
