
namespace KDS.e8086.Instructions
{
    internal abstract class RepeatableInstruction : Instruction
    {
        protected long RepeatCount;

        public RepeatableInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            RepeatCount = 0;
            if (EU.RepeatMode == RepeatModeEnum.NoRepeat)
            {
                DoInstruction();
                RepeatCount++;
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
                    RepeatCount++;
                    EU.Registers.CX--;
                } while (EU.Registers.CX != 0 && RepeatConditions());
            }

        }

        protected abstract void DoInstruction();

        protected virtual bool RepeatConditions()
        {
            return EU.RepeatMode != RepeatModeEnum.NoRepeat;
        }

    }
}
