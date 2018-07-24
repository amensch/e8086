﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    internal class MOV_WordImmediateToMemory : TwoByteInstruction
    {
        public MOV_WordImmediateToMemory(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        // MOV MEM-16, IMM-16
        // displacement bytes are optional so don't retrieve the immediate value
        // until the destination offset has been determined.

        protected override void ExecuteInstruction()
        {
            int dest;
            AssertMOD(secondByte.MOD);
            switch (secondByte.MOD)
            {
                case 0x00:
                    {
                        dest = GetRMTable1(secondByte.RM);
                        Bus.SaveWord(dest, GetImmediateWord());
                        break;
                    }
                case 0x01:
                    {
                        dest = GetRMTable2(secondByte.MOD, secondByte.RM);
                        Bus.SaveWord(dest, GetImmediateWord());
                        break;
                    }
                case 0x02:
                    {
                        dest = GetRMTable2(secondByte.MOD, secondByte.RM);
                        Bus.SaveWord(dest, GetImmediateWord());
                        break;
                    }
                case 0x03:
                    {
                        SaveWordToRegisters(secondByte.RM, GetImmediateWord());
                        break;
                    }
            }
        }
    }
}