﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    /// <summary>
    /// OpCodes b8-bf: MOV <reg>, IMM-16
    /// </summary>
    internal class MOV_ImmediateWord : Instruction
    {
        public MOV_ImmediateWord(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // use op code like the reg field to define the register
            byte reg = (byte)(OpCode & 0x07);
            ushort value = GetImmediateWord();
            EU.Registers.SaveRegisterValue(1, reg, value);
        }

    }
}
