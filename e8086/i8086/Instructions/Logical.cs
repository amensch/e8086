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
    public class Logical : TwoByteInstruction
    {
        public Logical(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
            // test op code to determine the operator
            switch ((byte)(OpCode >> 4))
            {
                case 0x00: // OR
                    {
                        OR_Destination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
                        break;
                    }
                case 0x02: // AND
                    {
                        AND_Destination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, false);
                        break;
                    }
                case 0x03: // XOR
                    {
                        XOR_Destination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
                        break;
                    }
                case 0x08: // TEST
                    {
                        AND_Destination(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, true);
                        break;
                    }
            }
        }

        protected void AND_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool test_only)
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
                    result = dest & source;
                    if (!test_only) SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest & source;
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
                            result = dest & source;
                            if (!test_only) Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest & source;
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
                                result = dest & source;
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

        protected void OR_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm)
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
                    result = dest | source;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest | source;
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
                            dest = Bus.GetData(word_size, offset);
                            result = dest | source;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest | source;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest | source;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = dest | source;
                                SaveRegField16(rm, (ushort)result);
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

        protected void XOR_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm)
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
                    result = dest ^ source;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest ^ source;
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
                            dest = Bus.GetData(word_size, offset);
                            result = dest ^ source;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest ^ source;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest ^ source;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = dest ^ source;
                                SaveRegField16(rm, (ushort)result);
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
