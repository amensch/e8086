﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// OpCode 0x6a: PUSH Imm-8
    /// </summary>
    internal class PUSH_ImmediateByte : Instruction
    {
        public PUSH_ImmediateByte(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            Push(EU.Bus.NextImmediate());
        }
    }
}
