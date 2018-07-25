using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal abstract class GroupInstruction : TwoByteInstruction
    {
        protected int source = 0;
        protected Dictionary<int, TwoByteInstruction> instructions;

        public GroupInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base (opCode, eu, bus)
        {
            instructions = new Dictionary<int, TwoByteInstruction>();
        }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            if(instructions.Count == 0)
            {
                LoadInstructionList();
            }
        }

        protected abstract void LoadInstructionList();

        protected override void ExecuteInstruction()
        {
            var instruction = instructions[secondByte.REG];
            instruction.SetSecondByte(secondByte.Value);
            instruction.Execute();
        }
    }
}
