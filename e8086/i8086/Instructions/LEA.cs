using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class LEA : TwoByteInstruction
    {
        public LEA(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // (0x8d) LEA MODREGR/M REG-16, MEM-16, (DISP-LO), (DISP-HI)
        // no flags
        // loads the offset of the source (rather than its value) and stores it in the destination

        protected override void ExecuteInstruction()
        {
            int offset = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            int source = Bus.GetData(wordSize, offset);
            SaveToDestination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        protected override void DetermineClocks()
        {
            Clocks = EffectiveAddressClocks + 2;
        }
    }
}
