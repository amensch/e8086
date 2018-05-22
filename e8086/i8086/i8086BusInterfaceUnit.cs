﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace KDS.e8086
{
    public class i8086BusInterfaceUnit
    {
        public const int MAX_MEMORY = 0x100000;

        /*
        From the 8086 manual
        Type              Default  Alternate  Offset
        --------------------------------------------
        Program counter      CS       --        IP
        Stack Op             SS       --        SP
        Variable             DS     CS,ES,SS    eff addr
        String src           DS     CS,ES,SS    SI
        String dest          ES       --        DI
        BP as Base           SS     CS,DS,ES    eff addr
        */

        public ushort CS { get; set; }  // code segment
        public ushort DS { get; set; }  // data segment
        public ushort SS { get; set; }  // stack segment
        public ushort ES { get; set; }  // extra segmemt
        public ushort IP { get; set; }  // instruction pointer

        public enum SegmentOverrideState
        {
            UseCS,
            UseDS,
            UseSS,
            UseES,
            NoOverride
        };

        public SegmentOverrideState SegmentOverride { get; set; }
        public bool UsingBasePointer { get; set; }

        private byte[] _ram;

        public i8086BusInterfaceUnit()
        {
            CS = 0xffff;
            DS = 0x0000;
            SS = 0x0000;
            ES = 0x0000;
            IP = 0x0000;
            SegmentOverride = SegmentOverrideState.NoOverride;
            UsingBasePointer = false;

            _ram = new byte[MAX_MEMORY];  // 1,048,576 bytes (maximum addressable by the 8086)
        }

        public void LoadBIOS(byte[] bios)
        {
            bios.CopyTo(_ram, MAX_MEMORY - bios.GetLength(0));
        }

        public void LoadROM(byte[] bios, int starting_address)
        {
            bios.CopyTo(_ram, starting_address);
        }

        // this is for testing
        public i8086BusInterfaceUnit(ushort startupCS, ushort startupIP, byte[] program)
        {
            CS = 0xffff;
            DS = 0x0000;
            SS = 0x0000;
            ES = 0x0000;
            IP = 0x0000;
            SegmentOverride = SegmentOverrideState.NoOverride;
            UsingBasePointer = false;

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

            int addr = GetPhysicalAddress();
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            program.CopyTo(_ram, addr);
        }

        // fetch the byte pointed to by the program counter and increment IP
        public byte NextIP()
        {
            int pc = GetPhysicalAddress();

            if (pc >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            byte mem = _ram[pc];
            IP++;
            return mem;
        }



        #region Get and Save data with segment calculation

        public int GetData(int word_size, int offset)
        {
            if (word_size == 0)
                return GetData8(offset);
            else
                return GetData16(offset);
        }

        public void SaveData(int word_size, int offset, int value)
        {
            if (word_size == 0)
                SaveData8(offset, (byte)value);
            else
                SaveData16(offset, (ushort)value);
        }

        // fetch the 8 bit value at the requested offset
        public byte GetData8(int offset)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return _ram[addr];
        }

        // save the 8 bit value to the requested offset
        public void SaveData8(int offset, byte value)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            _ram[addr] = value;
        }

        // fetch the 16 bit value at the requested offset
        public ushort GetData16(int offset)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return new DataRegister16(_ram[addr + 1], _ram[addr]);
        }

        // fetch the 16 bit value at the requested offset while forcing a segment address
        public ushort GetData16(int segment, int offset)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return new DataRegister16(_ram[addr + 1], _ram[addr]);
        }

        // save the 16 bit value to the requested offset
        public void SaveData16(int offset, ushort value)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            DataRegister16 data = new DataRegister16(value);
            _ram[addr + 1] = data.HI;
            _ram[addr] = data.LO;
        }

        #endregion

        #region Retrieve and Copy Strings
        // move string instruction
        // the source string segment can be overridden
        // the dest string segment is always ES
        public void MoveString8(int src_offset, int dst_offset)
        {
            _ram[(ES << 4) + dst_offset] = GetData8(src_offset);
        }

        public void MoveString16(int src_offset, int dst_offset)
        {
            int dst_addr = (ES << 4) + dst_offset;

            DataRegister16 data = new DataRegister16(GetData16(src_offset));
            _ram[dst_addr + 1] = data.HI;
            _ram[dst_addr] = data.LO;
        }

        public byte GetDestString8(int offset)
        {
            return _ram[(ES << 4) + offset];
        }

        public ushort GetDestString16(int offset)
        {
            int addr = (ES << 4) + offset;
            return new DataRegister16(_ram[addr + 1], _ram[addr]);
        }

        public void StoreString8(int offset, byte data)
        {
            _ram[(ES << 4) + offset] = data;
        }

        public void StoreString16(int offset, ushort data)
        {
            int addr = (ES << 4) + offset;
            DataRegister16 reg = new DataRegister16(data);
            _ram[addr + 1] = reg.HI;
            _ram[addr] = reg.LO;
        }

        #endregion

        #region Push and Pop
        public ushort PopStack(int offset)
        {
            int addr = (SS << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. SS={0:X4} offset={1:X4}", SS, offset));
            }

            return new DataRegister16(_ram[addr + 1], _ram[addr]); 
        }

        public void PushStack(int offset, ushort value)
        {
            int addr = (SS << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. SS={0:X4} offset={1:X4}", SS, offset));
            }

            DataRegister16 reg = new DataRegister16(value);
            _ram[addr + 1] = reg.HI;
            _ram[addr] = reg.LO;
        }

        #endregion

        private ushort GetDataSegment()
        {
            if (SegmentOverride == SegmentOverrideState.NoOverride)
            {
                if (UsingBasePointer)
                    return SS;
                else
                    return DS;
            }
            else if (SegmentOverride == SegmentOverrideState.UseCS)
                return CS;
            else if (SegmentOverride == SegmentOverrideState.UseES)
                return ES;
            else if (SegmentOverride == SegmentOverrideState.UseSS)
                return SS;
            else
                return DS;
        }

        private int GetPhysicalAddress()
        {
            return (CS << 4) + IP;
        }

        #region "Debugging helpers"

        public byte GetDataByPhysical(int idx)
        {
            if (idx >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. physaddr={0:X4}", idx));
            }
            return _ram[idx];
        }

        public byte GetOffsetFromIP(int offset)
        {
            int idx = GetPhysicalAddress() + offset;
            return GetDataByPhysical(idx);
        }

        public byte GetOffsetFromSP(int sp, int offset)
        {
            int idx = (SS << 4) + sp + offset;
            return GetDataByPhysical(idx);
        }

        public byte[] GetNext6Bytes()
        {
            return GetNextIPBytes(6);
        }

        // returns the next num bytes pointed to by the program counter
        // does not increment IP
        // the intended use is for on the fly disassembly
        public byte[] GetNextIPBytes(int num)
        {
            int pc = GetPhysicalAddress();
            byte[] next = new byte[num];

            Array.Copy(_ram, pc, next, 0, num);
            return next;
        }

        public byte[] GetNext6Physical(int idx)
        {
            byte[] next = new byte[6];
            int bytes = 6;

            if (idx + 6 >= MAX_MEMORY)
                bytes = (MAX_MEMORY - idx - 1);

            Array.Copy(_ram, idx, next, 0, bytes);
            return next;
        }

        #endregion
    }
}
