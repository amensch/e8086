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
            IInputDevice device;
            if(EU.TryGetInputDevice(port, out device))
            {
                if(wordSize == 0)
                {
                    EU.Registers.AL = device.ReadByte();
                }
                else
                {
                    EU.Registers.AX = device.ReadWord();
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
