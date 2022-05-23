
namespace KDS.e8086.Instructions
{
    /// <summary>
    /// DAS: Decimal Adjust Subtract
    /// </summary>
    internal class DAS : Instruction
    {
        public DAS(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            byte old_al = EU.Registers.AL;
            bool old_cf = EU.CondReg.CarryFlag;

            if ((EU.Registers.AL & 0x0f) > 9 || EU.CondReg.AuxCarryFlag)
            {
                EU.CondReg.CalcCarryFlag(0, EU.Registers.AL - 6);
                EU.CondReg.CarryFlag = old_cf | EU.CondReg.CarryFlag;
                EU.CondReg.AuxCarryFlag = true;

                EU.Registers.AL -= 6;
            }
            else
            {
                EU.CondReg.AuxCarryFlag = false;
            }

            if ((old_al > 0x99) | old_cf)
            {
                EU.Registers.AL -= 0x60;
                EU.CondReg.CarryFlag = true;
            }
            else
            {
                EU.CondReg.CarryFlag = false;
            }

            EU.CondReg.CalcSignFlag(0, EU.Registers.AL);
            EU.CondReg.CalcZeroFlag(0, EU.Registers.AL);
            EU.CondReg.CalcParityFlag(EU.Registers.AL);
        }

        public override long Clocks()
        {
            return 4;
        }

    }
}
