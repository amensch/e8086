using System;

namespace KDS.Utility
{
    public class Util
    {
        public static UInt16 GetValue16(byte hi, byte lo)
        {
            return (UInt16)(((UInt32)hi << 8 | (UInt32)lo) & 0xffff);
        }
    }
}
