﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    /// <summary>
    /// 
    /// </summary>
    public class POP : TwoByteInstruction
    {
        public POP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            // parse the op code byte into r/m components
            var opByte = new AddressMode(OpCode);

            // for segment register ops, use the reg field
            if (OpCode < 0x50)
            {
                SaveSegRegField(opByte.REG, Pop());
            }
            // else use rm field to determine the register
            else
            {
                SaveRegField16(opByte.RM, Pop());
            }
        }


    }

}
