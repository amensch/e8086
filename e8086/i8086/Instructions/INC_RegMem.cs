using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class INC_RegMem : TwoByteInstruction
    {
        public INC_RegMem(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = 1;
            int dest = GetDestinationData(0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int result = dest + 1;

            SaveToDestination(result, 0, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);

            // When calculating flags the word size is always 16
            // TODO: IS THIS CORRECT?

            // Flags: O S Z A P
            // Flags are set as if ADD or SUB instruction was used with operand2 = 1
            // Carry flag is not affected by increment
            EU.CondReg.CalcOverflowFlag(1, source, dest);
            EU.CondReg.CalcSignFlag(1, result);
            EU.CondReg.CalcZeroFlag(1, result);
            EU.CondReg.CalcAuxCarryFlag(source, dest);
            EU.CondReg.CalcParityFlag(result);
        }

        protected override void DetermineClocks()
        {
            if (secondByte.MOD == 0x03)
            {
                Clocks = 3;
            }
            else
            {
                // memory + EA
                Clocks = EffectiveAddressClocks + 15;
            }
        }
    }
}
