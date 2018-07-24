using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal abstract class RepeatableInstruction : Instruction
    {
        public RepeatableInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            if (EU.RepeatMode == RepeatModeEnum.NoRepeat)
            {
                DoInstruction();
            }
            else if(EU.RepeatMode != RepeatModeEnum.NoRepeat && EU.Registers.CX == 0)
            {
                // If repeat is on but CX is 0 do not carry out the instruction.
            }
            else
            {
                do
                {
                    DoInstruction();
                    EU.Registers.CX--;
                } while (EU.Registers.CX != 0 && RepeatConditions());
            }

        }

        protected abstract void DoInstruction();

        protected virtual bool RepeatConditions()
        {
            return (EU.RepeatMode != RepeatModeEnum.NoRepeat);
        }

    }
}
