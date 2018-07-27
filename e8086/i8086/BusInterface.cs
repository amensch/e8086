using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace KDS.e8086
{
    public class BusInterface : IBus
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

        public SegmentOverrideState SegmentOverride { get; set; }
        public bool UsingBasePointer { get; set; }

        public RAM Ram { get; set; }

        public BusInterface()
        {
            CS = 0xffff;
            DS = 0x0000;
            SS = 0x0000;
            ES = 0x0000;
            IP = 0x0000;
            SegmentOverride = SegmentOverrideState.NoOverride;
            UsingBasePointer = false;

            Ram = new RAM(MAX_MEMORY);  // 1,048,576 bytes (maximum addressable by the 8086)
        }

        public void LoadBIOS(byte[] bios)
        {
            Ram.Load(bios, MAX_MEMORY - bios.GetLength(0));
        }

        public void LoadROM(byte[] bios, int starting_address)
        {
            Ram.Load(bios, starting_address);
        }

        // this is for testing
        public BusInterface(ushort startupCS, ushort startupIP, byte[] program)
        {
            CS = 0xffff;
            DS = 0x0000;
            SS = 0x0000;
            ES = 0x0000;
            IP = 0x0000;
            SegmentOverride = SegmentOverrideState.NoOverride;
            UsingBasePointer = false;

            Ram = new RAM(MAX_MEMORY);  // 1,048,576 bytes (maximum addressable by the 8086)

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

            Ram.Load(program, addr);
        }

        // fetch the byte pointed to by the program counter and increment IP
        public byte NextImmediate()
        {
            int pc = GetPhysicalAddress();

            if (pc >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. CS={0:X4} IP={1:X4}", CS, IP));
            }

            byte mem = Ram[pc];
            IP = (ushort)((IP + 1) & 0xffff);
            return mem;
        }



        #region Get and Save data with segment calculation

        public int GetData(int word_size, int offset)
        {
            if (word_size == 0)
                return GetByte(offset);
            else
                return GetWord(offset);
        }

        public void SaveData(int word_size, int offset, int value)
        {
            if (word_size == 0)
                SaveByte(offset, (byte)value);
            else
                SaveWord(offset, (ushort)value);
        }

        // fetch the 8 bit value at the requested offset
        private byte GetByte(int offset)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return Ram[addr];
        }

        // save the 8 bit value to the requested offset
        public void SaveByte(int offset, byte value)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            Ram[addr] = value;
        }

        // fetch the 16 bit value at the requested offset
        private ushort GetWord(int offset)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return new WordRegister(Ram[addr + 1], Ram[addr]);
        }

        // fetch the 16 bit value at the requested offset while forcing a segment address
        public ushort GetWord(int segment, int offset)
        {
            int addr = (segment << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            return new WordRegister(Ram[addr + 1], Ram[addr]);
        }

        // save the 16 bit value to the requested offset
        public void SaveWord(int offset, ushort value)
        {
            int addr = (GetDataSegment() << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. DS={0:X4} offset={1:X4}", DS, offset));
            }
            WordRegister data = new WordRegister(value);
            Ram[addr + 1] = data.HI;
            Ram[addr] = data.LO;
        }

        #endregion

        #region Retrieve and Copy Strings
        // move string instruction
        // the source string segment can be overridden
        // the dest string segment is always ES
        public void MoveByteString(int src_offset, int dst_offset)
        {
            Ram[(ES << 4) + dst_offset] = GetByte(src_offset);
        }

        public void MoveWordString(int src_offset, int dst_offset)
        {
            int dst_addr = (ES << 4) + dst_offset;

            WordRegister data = new WordRegister(GetWord(src_offset));
            Ram[dst_addr + 1] = data.HI;
            Ram[dst_addr] = data.LO;
        }

        public int GetDestString(int word_size, int offset)
        {
            if (word_size == 0)
                return GetByteDestString(offset);
            else
                return GetWordDestString(offset);
        }

        public byte GetByteDestString(int offset)
        {
            return Ram[(ES << 4) + offset];
        }

        public ushort GetWordDestString(int offset)
        {
            int addr = (ES << 4) + offset;
            return new WordRegister(Ram[addr + 1], Ram[addr]);
        }

        public void SaveByteString(int offset, byte data)
        {
            Ram[(ES << 4) + offset] = data;
        }

        public void SaveWordString(int offset, ushort data)
        {
            int addr = (ES << 4) + offset;
            WordRegister reg = new WordRegister(data);
            Ram[addr + 1] = reg.HI;
            Ram[addr] = reg.LO;
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

            return new WordRegister(Ram[addr + 1], Ram[addr]); 
        }

        public void PushStack(int offset, ushort value)
        {
            int addr = (SS << 4) + offset;
            if (addr >= MAX_MEMORY)
            {
                throw new InvalidOperationException(String.Format("Memory bounds exceeded. SS={0:X4} offset={1:X4}", SS, offset));
            }

            WordRegister reg = new WordRegister(value);
            Ram[addr + 1] = reg.HI;
            Ram[addr] = reg.LO;
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
            return Ram[idx];
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
            return Ram.GetChunk(GetPhysicalAddress(), num);
        }

        #endregion
    }
}
