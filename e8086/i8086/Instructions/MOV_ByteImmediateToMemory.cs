using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class MOV_ByteImmediateToMemory : TwoByteInstruction
    {
        public MOV_ByteImmediateToMemory(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int dest;
            AssertMOD(secondByte.MOD);
            switch (secondByte.MOD)
            {
                case 0x00:
                    {
                        dest = GetRMTable1(secondByte.RM);
                        Bus.SaveByte(dest, Bus.NextIP());
                        break;
                    }
                case 0x01:
                    {
                        dest = GetRMTable2(secondByte.MOD, secondByte.RM);
                        Bus.SaveByte(dest, Bus.NextIP());
                        break;
                    }
                case 0x02:
                    {
                        dest = GetRMTable2(secondByte.MOD, secondByte.RM);
                        Bus.SaveByte(dest, Bus.NextIP());
                        break;
                    }
                case 0x03:
                    {
                        SaveByteToRegisters(secondByte.RM, Bus.NextIP());
                        break;
                    }
            }
        }
    }
}
