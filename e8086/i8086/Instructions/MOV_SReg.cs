using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class MOV_SReg : TwoByteInstruction
    {
        public MOV_SReg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, true);
            SaveToDestination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, true);
        }
    }
}
