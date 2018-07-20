using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public abstract class OutputInstruction : Instruction
    {
        protected ushort port = 0;
        public OutputInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            IODevice device;
            if (EU.TryGetDevice(port, out device))
            {
                device.writeData(wordSize, EU.Registers.AX);
            }
        }
    }
}
