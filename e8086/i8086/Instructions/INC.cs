
namespace KDS.e8086.Instructions
{
    /// <summary>
    /// OpCodes 0x40 - 0x47
    /// Increment the designated register.
    /// Parse the op code, the affected register is the last 3 bits.
    /// </summary>
    internal class INC : Instruction
    {
        public INC(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // always use word size = 1
            ushort dest = (ushort)(EU.Registers.GetRegisterValue(1, OpCodeMode.RM) & 0xffff);
            ushort result = (ushort)(dest + 1);
            EU.Registers.SaveRegisterValue(1, OpCodeMode.RM, result);

            // Flags: O S Z A P
            // Flags are set as if ADD instruction was used with operand2 = 1
            // Carry flag is not affected by increment
            EU.CondReg.CalcOverflowFlag(1, 0x01, dest);
            EU.CondReg.CalcSignFlag(1, result);
            EU.CondReg.CalcZeroFlag(1, result);
            EU.CondReg.CalcAuxCarryFlag(0x01, dest);
            EU.CondReg.CalcParityFlag(result);
        }
    }
}
