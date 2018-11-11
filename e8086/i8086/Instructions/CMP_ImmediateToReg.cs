﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal class CMP_ImmediateToReg : SUB_ImmediateToReg
    {
        public CMP_ImmediateToReg(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus)
        {
            SubWithBorrow = false;
            CompareOnly = false;
        }

        protected override void DetermineClocks()
        {
            if (secondByte.MOD == 0x03)
            {
                // reg,imm
                Clocks = 4;
            }
            else
            {
                // mem,imm
                Clocks = EffectiveAddressClocks + 10;
            }
        }
    }
}
