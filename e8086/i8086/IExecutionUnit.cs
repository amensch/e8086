﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public enum RepeatModeEnum
    {
        NoRepeat,
        REP,
        REPNZ
    }
    public interface IExecutionUnit
    {
        IBus Bus { get; }
        Registers Registers { get; }
        ConditionalRegister CondReg { get; }

        bool Halted { get; set; }

        int ReadPort(int wordSize, ushort port);
        void WritePort(int wordSize, ushort port, int data);

        /// <summary>
        /// Mode Value
        /// 0 = no repeat is in effect
        /// 1 = repeat until zero flag is false
        /// 2 = repeat until zero flag is true
        /// </summary>
        RepeatModeEnum RepeatMode { get; set; }

        long InstructionCount { get; } 
    }
}
