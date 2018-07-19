using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public abstract class LoadSegmentInstruction : TwoByteInstruction
    {
        // Transfer a 32 bit pointer variable from the source operand (which must be memory)
        // to the destination operand and DS.
        // The offset word of the pointer is transferred to the destination operand.
        // The segment word of the pointer is transferred to register DS.

        /*
            0xc4 LES reg-16, mem-16
            0xc5 LDS reg-16, mem-16
                 OP MODREGR/M (DISP-LO) (DISP-HI)
        */

        protected int offset = 0;

        public LoadSegmentInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            AssertMOD(secondByte.MOD);
            switch (secondByte.MOD)
            {
                case 0x00:
                    {
                        offset = Bus.GetWord(GetRMTable1(secondByte.RM));
                        break;
                    }
                case 0x01:
                case 0x02:   // difference is processed in the GetRMTable2 function
                    {
                        offset = Bus.GetWord(GetRMTable2(secondByte.MOD, secondByte.RM));
                        break;
                    }
                case 0x03:
                    {
                        throw new ArgumentOutOfRangeException("mod", secondByte.MOD, string.Format("Invalid mod value in opcode={0:X2}", OpCode));
                    }
            }

            SaveWordToRegisters(secondByte.REG, Bus.GetWord(offset));
        }

        protected override void ExecuteInstruction()
        {
            base.ExecuteInstruction();
        }
    }
}
