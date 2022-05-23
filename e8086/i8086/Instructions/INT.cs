
namespace KDS.e8086.Instructions
{
    internal class INT : Instruction
    {
        private int InterruptNumber = 0;
        private bool DoInterrupt;

        public INT(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            DoInterrupt = true;
        }

        public INT(byte opCode, int interruptNumber, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            DoInterrupt = true;
            InterruptNumber = interruptNumber;
        }

        public INT(byte opCode, bool interruptCondition, int interruptNumber, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            DoInterrupt = interruptCondition;
            InterruptNumber = interruptNumber;
        }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            if(InterruptNumber == 0)
            {
                InterruptNumber = EU.Bus.NextImmediate();
            }
        }

        protected override void ExecuteInstruction()
        {
            if (DoInterrupt)
            {
                // push flags
                Push(EU.CondReg.Value);

                // push CS and IP
                Push(Bus.CS);
                Push(Bus.IP);

                // clear trap flag
                EU.CondReg.TrapFlag = false;

                // clear interrupt enable
                EU.CondReg.InterruptEnable = false;

                // replace IP by first word of interrupt pointer
                Bus.IP = Bus.GetWord(0, InterruptNumber * 4);

                // the second word of the interrupt pointer replaces CS
                Bus.CS = Bus.GetWord(0, (InterruptNumber * 4) + 2);
            }
        }

        public override long Clocks()
        {
            if(InterruptNumber == 3)
            {
                return 52;
            }
            else
            {
                return 51;
            }
        }
    }
}
