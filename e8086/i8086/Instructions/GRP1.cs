using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class GRP1 : GroupInstruction
    {
        public GRP1(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void LoadInstructionList()
        {
            instructions.Add(0x00, new ADD_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x01, new OR_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x02, new ADC_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x03, new SBB_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x04, new AND_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x05, new SUB_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x06, new XOR_ImmediateToReg(OpCode, EU, Bus));
            instructions.Add(0x07, new CMP_ImmediateToReg(OpCode, EU, Bus));
        }

    }
}
