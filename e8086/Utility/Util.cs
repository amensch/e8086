using System;

namespace KDS.Utility
{
    public class Util
    {

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

        public static int GetDirection(byte b)
        {
            return (b >> 1) & 0x01;
        }

        public static int GetWordSize(byte b)
        {
            return b & 0x01;
        }

        // convert a segment and offset into a physical ram address
        public static int ConvertLogicalToPhysical(UInt16 segment, UInt16 offset)
        {
            return (segment << 4) + offset;
        }
    }
}
