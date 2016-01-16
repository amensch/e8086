﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class DataRegister16
    {
        public byte HI { get; set; }
        public byte LO { get; set; }

        public UInt16 Register
        {
            get
            {
                return (UInt16)(((UInt32)HI << 8 | (UInt32)LO) & 0xffff);
            }
            set
            {
                HI = (byte)(value >> 8);
                LO = (byte)(value & 0x00ff);
            }
        }
    }
}