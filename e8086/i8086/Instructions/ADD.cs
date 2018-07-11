using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// Op Codes: 00-03, 10-13
    /// operand1 = operand1 + operand2
    /// Flags: O S Z A P C    
    /// </summary>
    public class ADD : TwoByteInstruction
    {
        protected bool AddWithCarry;

        public ADD(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            AddWithCarry = false;
        }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            ADD_Destination(source, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        protected void ADD_Destination(int source, byte mod, byte reg, byte rm)
        {
            AssertMOD(mod);
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
                if (wordSize == 0)
                {
                    dest = GetRegField8(reg);
                    result = source + dest + carry;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = source + dest + carry;
                    SaveRegField16(reg, (ushort)result);
                }
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
                            if (wordSize == 0)
                            {
                                dest = GetRegField8(rm);
                                result = source + dest + carry;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = source + dest + carry;
                                SaveRegField16(rm, (ushort)result);
                            }
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
