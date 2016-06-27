using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDS.Utility;

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

            int addr = Util.ConvertLogicalToPhysical(CS, IP);
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            program.CopyTo(_ram, addr);
        }

        // fetch the byte pointed to by the program counter
        public byte NextIP()
        {
            int pc = Util.ConvertLogicalToPhysical(CS, IP);

            if( pc >= MAX_MEMORY )
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            byte mem = _ram[pc];
            IP++;
            return mem;
        }

        // fetch the 8 bit value at the requested offset
        public byte GetData8(UInt16 offset)
        {
            int addr = (UInt16)((DS << 4) + offset);
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return _ram[addr];
        }

        // save the 8 bit value at the requested offset
        public void SaveData8(UInt16 offset, byte value)
        {
            int addr = (UInt16)((DS << 4) + offset);
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            _ram[addr] = value;
        }

        // fetch the 16 bit value at the requested offset
        public UInt16 GetData16(UInt16 offset)
        {
            int addr = (UInt16)((DS << 4) + offset);
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return Util.GetValue16(_ram[addr + 1], _ram[addr]);
        }

        // save the 16 bit value at the requested offset
        public void SaveData16(UInt16 offset, UInt16 value)
        {
            int addr = (UInt16)((DS << 4) + offset);
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            
        }

        //// retrieve 16 bit value from a physical memory location
        //public UInt16 GetFromRAM(int addr)
        //{
        //    return Util.GetValue16(_ram[addr + 1], _ram[addr]);
        //}

        //// retrieve 16 bit value from a logical memory location
        //public UInt16 GetFromRAM(UInt16 segment, UInt16 offset)
        //{
        //    return GetFromRAM(Util.ConvertLogicalToPhysical(segment, offset));
        //}

        //// retrieve 16 bit value which is pointed to by the given physical address
        //public UInt16 GetFromPointer(int addr)
        //{
        //    return GetFromRAM(GetFromRAM(addr + 2), GetFromRAM(addr));
        //}

        //// retrieve 16 bit value which is pointed to by the given logical address
        //public UInt16 GetFromPointer(UInt16 segment, UInt16 offset)
        //{
        //    return GetFromPointer(Util.ConvertLogicalToPhysical(segment, offset));
        //}
    }
}
