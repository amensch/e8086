﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class NOOP : Instruction
    {
        public NOOP(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }
    }
}