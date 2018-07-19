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
            IOutputDevice device;
            if (EU.TryGetOutputDevice(port, out device))
            {
                if (wordSize == 0)
                {
                    device.Write(EU.Registers.AL);
                }
                else
                {
                    device.Write(EU.Registers.AX);
                }
            }
        }
    }
}
