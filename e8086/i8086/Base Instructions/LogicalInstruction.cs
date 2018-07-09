using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// Logical operations on reg/mem
    /// OR: 08-0B
    /// AND: 20-23
    /// XOR: 30-33
    /// TEST: 84-85
    /// </summary>
    public abstract class LogicalInstruction : TwoByteInstruction
    {
        protected int source;
        public LogicalInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        protected abstract int Operand(int source, int dest);

        protected void ProcessInstruction(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool test_only)
        {
            AssertMOD(mod);
            int result = 0;
            int offset;
            int dest = 0;

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (word_size == 0)
                {
                    dest = GetRegField8(reg);
                    result = Operand(source, dest);
                    if (!test_only) SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = Operand(source, dest);
                    if (!test_only) SaveRegField16(reg, (ushort)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(word_size, offset);
                            result = Operand(source, dest);
                            if (!test_only) Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = Operand(source, dest);
                            if (!test_only) Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest & source;
                                if (!test_only) SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = Operand(source, dest);
                                if (!test_only) SaveRegField16(rm, (ushort)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            EU.CondReg.OverflowFlag = false;
            EU.CondReg.CarryFlag = false;
            EU.CondReg.CalcSignFlag(word_size, result);
            EU.CondReg.CalcZeroFlag(word_size, result);
            EU.CondReg.CalcParityFlag(result);

        }
    }
}
