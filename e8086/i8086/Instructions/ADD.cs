
namespace KDS.e8086.Instructions
{
    /// <summary>
    /// Op Codes: 00-03, 10-13
    /// operand1 = operand1 + operand2
    /// Flags: O S Z A P C    
    /// </summary>
    internal class ADD : TwoByteInstruction
    {
        protected virtual bool AddWithCarry => false;

        public ADD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
        }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            ADD_Destination(source, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        public override long Clocks()
        {
            // reg,reg
            if (secondByte.MOD == 0x03)
            {
                return 3;
            }
            // mem,reg
            else if (direction == 0)
            {
                return EffectiveAddressClocks + 16;
            }
            // reg,mem
            else
            {
                return EffectiveAddressClocks + 9;
            }
        }

        protected void ADD_Destination(int source, byte mod, byte reg, byte rm)
        {
            int result = 0;
            int offset;
            int dest = 0;
            int carry = 0;

            // Include carry flag if necessary
            if (AddWithCarry && EU.CondReg.CarryFlag)
            {
                carry = 1;
            }

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                dest = EU.Registers.GetRegisterValue(wordSize, reg);
                result = source + dest + carry;
                EU.Registers.SaveRegisterValue(wordSize, reg, result);
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(wordSize, offset);
                            result = source + dest + carry;
                            Bus.SaveData(wordSize, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(wordSize, offset);
                            result = source + dest + carry;
                            Bus.SaveData(wordSize, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            dest = EU.Registers.GetRegisterValue(wordSize, rm);
                            result = source + dest + carry;
                            EU.Registers.SaveRegisterValue(wordSize, rm, result);
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            EU.CondReg.CalcOverflowFlag(wordSize, source, dest);
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcAuxCarryFlag(source, dest);
            EU.CondReg.CalcParityFlag(result);
            EU.CondReg.CalcCarryFlag(wordSize, result);
        }

    }
}
