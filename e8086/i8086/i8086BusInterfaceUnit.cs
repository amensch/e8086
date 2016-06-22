using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086BusInterfaceUnit
    {
        private const int MAX_MEMORY = 0x100000;

        public UInt16 CS { get; set; }  // code segment
        public UInt16 DS { get; set; }  // data segment
        public UInt16 SS { get; set; }  // stack segment
        public UInt16 ES { get; set; }  // extra segmemt
        public UInt16 IP { get; set; }  // instruction pointer

        private byte[] _ram;

        public i8086BusInterfaceUnit(UInt16 startupCS, UInt16 startupIP, byte[] program)
        {
            CS = 0xffff;
            DS = 0x0000;
            SS = 0x0000;
            ES = 0x0000;
            IP = 0x0000;

            _ram = new byte[MAX_MEMORY];  // 1,048,576 bytes (maximum addressable by the 8086)

            // On bootup the architecture is hard coded to look at memory location 0xffff0 (FFFF:0000). This is the reset vector that
            // contains a 16 byte address.  The CPU will jump to this address and begin executing.  On a PC this will be
            // an address within ROM so the system can start up.
            //
            // For simplicity sake for small programs one could simplify things by having CS=DS=SS so all code, 
            // data, and the stack are contained within a single 64KB segment.
            //
            // In simple terms the location 0xffff0 contains a four byte address.  The first 16 bit value is the
            // new value of the code segment and the second 16 bit value is the instruction pointer.

            CS = startupCS;
            IP = startupIP;

            int addr = GetAddress(CS, IP);
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            program.CopyTo(_ram, addr);
        }

        // fetch the byte pointed to by the program counter
        public byte NextIP()
        {
            int pc = GetAddress(CS, IP);

            if( pc >= MAX_MEMORY )
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            byte mem = _ram[pc];
            IP++;
            return mem;
        }

        // calculate the physical address into RAM
        private int GetAddress(UInt16 segment, UInt16 offset)
        {
            return (segment << 4) + offset;
        }
    }
}
