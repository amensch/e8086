using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public abstract class InputInstruction : Instruction
    {
        protected ushort port = 0;
        public InputInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            IODevice device;
            if(EU.TryGetDevice(port, out device))
            {
                if(wordSize == 0)
                {
                    EU.Registers.AL = (byte)device.ReadData(wordSize);
                }
                else
                {
                    EU.Registers.AX = (ushort)device.ReadData(wordSize);
                }
            }
            else
            {
                // if no device is attached zero the register
                if (wordSize == 0)
                {
                    EU.Registers.AL = 0;
                }
                else
                {
                    EU.Registers.AX = 0;
                }
            }
        }
    }
}
