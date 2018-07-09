﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class OR : LogicalInstruction
    {
        public OR(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            ProcessInstruction(source, direction, wordSize, secondByte.MOD, secondByte.REG, secondByte.RM, false);
        }

        protected override int Operand(int source, int dest)
        {
            return (dest | source);
        }
    }
}