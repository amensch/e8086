using System;

namespace KDS.Utility
{
    public class Util
    {
        public static UInt16 GetValue16(byte hi, byte lo)
        {
            return (UInt16)(((UInt32)hi << 8 | (UInt32)lo) & 0xffff);
        }

        public static void SplitValue16(UInt16 num, ref byte hi, ref byte lo)
        {
            hi = (byte)((num >> 8) & 0x00ff);
            lo = (byte)(num & 0x00ff);
        }

        public static byte GetMODValue(byte b)
        {
            return (byte)((b >> 6) & 0x03);
        }

        public static byte GetREGValue(byte b)
        {
            return (byte)((b >> 3) & 0x07);
        }

        public static byte GetRMValue(byte b)
        {
            return (byte)(b & 0x07);
        }

        // convert a segment and offset into a physical ram address
        public static int ConvertLogicalToPhysical(UInt16 segment, UInt16 offset)
        {
            return (segment << 4) + offset;
        }
    }
}
