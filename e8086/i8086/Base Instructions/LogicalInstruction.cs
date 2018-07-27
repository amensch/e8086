﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// Logical operations on reg/mem
    /// OR: 08-0B
    /// AND: 20-23
    /// XOR: 30-33
    /// TEST: 84-85
    /// </summary>
    internal abstract class LogicalInstruction : TwoByteInstruction
    {
        protected int source;
        public LogicalInstruction(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void PreProcessing()
        {
            base.PreProcessing();
            source = GetSourceData(direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM);
        }

        protected abstract int Operand(int source, int dest);

        protected void ProcessInstruction(int source, byte mod, byte reg, byte rm, bool test_only)
        {
            int result = 0;
            int offset;
            int dest = 0;

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                dest = EU.Registers.GetRegisterValue(wordSize, reg);
                result = Operand(source, dest);
                if (!test_only) EU.Registers.SaveRegisterValue(wordSize, reg, result);
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(wordSize, offset);
                            result = Operand(source, dest);
                            if (!test_only) Bus.SaveData(wordSize, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(wordSize, offset);
                            result = Operand(source, dest);
                            if (!test_only) Bus.SaveData(wordSize, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            dest = EU.Registers.GetRegisterValue(wordSize, rm);
                            result = Operand(source, dest);
                            if (!test_only) EU.Registers.SaveRegisterValue(wordSize, rm, result);
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            EU.CondReg.OverflowFlag = false;
            EU.CondReg.CarryFlag = false;
            EU.CondReg.CalcSignFlag(wordSize, result);
            EU.CondReg.CalcZeroFlag(wordSize, result);
            EU.CondReg.CalcParityFlag(result);

        }
    }
}
