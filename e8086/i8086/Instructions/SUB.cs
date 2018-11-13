using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// Op Codes: SUB 28-2B, SBB 18-1B
    ///         : CMP 38-3B
    /// operand1 = operand1 - operand2
    /// Flags: O S Z A P C
    /// </summary>
    internal class SUB : TwoByteInstruction
    {
        protected bool SubWithBorrow;
        protected bool CompareOnly;

        public SUB(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            SubWithBorrow = false;
            CompareOnly = false;
        }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            SUB_Destination(source, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        protected void SUB_Destination(int source, byte mod, byte reg, byte rm)
        {
            int result = 0;
            int offset;
            int dest = 0;
            int carry = 0;

            // Include carry flag if necessary
            if (SubWithBorrow && EU.CondReg.CarryFlag)
            {
                carry = 1;
            }

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                dest = EU.Registers.GetRegisterValue(wordSize, reg);
                result = dest - (source + carry);
                if (!CompareOnly) EU.Registers.SaveRegisterValue(wordSize, reg, result);
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(wordSize, offset);
                            result = dest - (source + carry);
                            if (!CompareOnly) Bus.SaveData(wordSize, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(wordSize, offset);
                            result = dest - (source + carry);
                            if (!CompareOnly) Bus.SaveData(wordSize, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            dest = EU.Registers.GetRegisterValue(wordSize, rm);
                            result = dest - (source + carry);
                            if (!CompareOnly) EU.Registers.SaveRegisterValue(wordSize, rm, result);
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            EU.CondReg.CalcOverflowSubtract(wordSize, source + carry, dest);
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcAuxCarryFlag(source, dest);
            EU.CondReg.CalcParityFlag(result);
            EU.CondReg.CalcCarryFlag(wordSize, result);
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
    }
}
