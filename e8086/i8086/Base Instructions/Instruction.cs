using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086.Instructions
{
    internal abstract class Instruction
    {
        public byte OpCode { get; private set; }
        protected AddressMode OpCodeMode { get; private set; }

        protected IExecutionUnit EU { get; private set; }
        protected IBus Bus { get; private set; }
        protected int direction;
        protected int wordSize;

        private long LastEACount = 0;
        private ushort LastEAResult = 0;

        /// <summary>
        /// Count the numbber of clocks necessary for the instruction.
        /// 
        /// NOTES:
        /// 
        /// On an 8086:
        /// Add 4 clocks for each reference to a word operand located at an odd memory address.
        /// 
        /// On an 8088:
        /// Add 4 clocks for each reference to a 16 bit memory operation including the stack
        /// 
        /// Effective address Calculation Time:
        /// Displacement Only                   6 clocks
        /// Base or Index Only (BX,BP,SI,DI)    5 clocks
        /// Displacement + Index (BX,BP,SI,DI)  9 clocks
        /// Base + Index (BP+DI, BX+SI)         7 clocks
        /// Base + Index (BP+SI, BX+DI)         8 clocks
        /// Disp+Base+Index (BP+DI+DISP)       11 clocks
        /// Disp+Base+Index (BX+SI+DISP)       11 clocks
        /// Disp+Base+Index (BP+SI+DISP)       12 clocks
        /// Disp+Base+Index (BX+DI+DISP)       12 clocks
        /// </summary>
        protected long EffectiveAddressClocks { get; set; }

        public Instruction(byte opCode, IExecutionUnit eu, IBus bus)
        {
            OpCode = opCode;
            EU = eu;
            Bus = bus;
            direction = (OpCode >> 1) & 0x01;
            wordSize = (OpCode & 0x01);
            OpCodeMode = new AddressMode(opCode);
        }

        protected virtual void PreProcessing() { }
        protected virtual void ExecuteInstruction() { }

        public void Execute()
        {
            EffectiveAddressClocks = 0;
            PreProcessing();
            ExecuteInstruction();
        }

        public virtual long Clocks()
        {
            throw new NotImplementedException();
        }

        #region Helper and Utility functions

        // Gets the immediate 16 bit value
        protected ushort GetImmediateWord()
        {
            byte lo = Bus.NextImmediate();
            byte hi = Bus.NextImmediate();
            return new WordRegister(hi, lo);
        }

        // Sign extend 8 bits to 16 bits
        protected ushort SignExtendByteToWord(byte num)
        {
            if (num < 0x80)
                return num;
            else
                return new WordRegister(0xff, num);
        }

        // Sign extend 16 bits to 32 bits
        protected uint SignExtendWordToDW(ushort num)
        {
            if (num < 0x8000)
                return num;
            else
                return num | 0xffffff00;
        }

        #endregion

        #region Push and Pop
        protected void Push(ushort value)
        {
            EU.Registers.SP -= 2;
            Bus.PushStack(EU.Registers.SP, value);
        }

        protected ushort Pop()
        {
            ushort result = Bus.PopStack(EU.Registers.SP);
            EU.Registers.SP += 2;
            return result;
        }
        #endregion

        #region Get and Set  segmentation registers

        // Get 16 bit SREG result
        protected ushort GetWordFromSegReg(byte reg)
        {
            ushort result = 0;
            switch (reg)
            {
                case 0x00:
                    {
                        result = Bus.ES;
                        break;
                    }
                case 0x01:
                    {
                        result = Bus.CS;
                        break;
                    }
                case 0x02:
                    {
                        result = Bus.SS;
                        break;
                    }
                case 0x03:
                    {
                        result = Bus.DS;
                        break;
                    }
            }
            return result;
        }

        // Save 16 bit value into a Seg Reg
        protected void SaveWordToSegReg(byte reg, ushort value)
        {
            switch (reg)
            {
                case 0x00:
                    {
                        Bus.ES = value;
                        break;
                    }
                case 0x01:
                    {
                        Bus.CS = value;
                        break;
                    }
                case 0x02:
                    {
                        Bus.SS = value;
                        break;
                    }
                case 0x03:
                    {
                        Bus.DS = value;
                        break;
                    }
            }
        }
        #endregion

        #region Generic Retrieve and Store functions
        // Generic function to retrieve source data based on the op code and addr byte
        protected int GetSourceData(int direction, int word_size, byte mod, byte reg, byte rm)
        {
            return GetSourceData(direction, word_size, mod, reg, rm, false);
        }

        protected int GetSourceData(int direction, int word_size, byte mod, byte reg, byte rm, bool useSREG)
        {
            int result = 0;

            // Action is the same if the direction is 0 (source is REG)
            if (direction == 0)
            {
                if (useSREG)
                {
                    result = GetWordFromSegReg(reg);
                }
                else
                {
                    result = EU.Registers.GetRegisterValue(word_size, reg);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            result = Bus.GetData(useSREG ? 1 : word_size, GetRMTable1(rm));
                            break;
                        }
                    case 0x01:
                    case 0x02:   // difference is processed in the GetRMTable2 function
                        {
                            result = Bus.GetData(useSREG ? 1 : word_size, GetRMTable2(mod, rm));
                            break;
                        }
                    case 0x03:
                        {
                            result = EU.Registers.GetRegisterValue(useSREG ? 1 : word_size, rm);
                            break;
                        }
                }
            }
            return result;
        }

        // Generic function to retrieve the current data in the destination based on the op code and addr byte
        protected int GetDestinationData(int direction, int word_size, byte mod, byte reg, byte rm)
        {
            if (direction == 0)
            {
                return GetSourceData(1, word_size, mod, reg, rm);
            }
            else
            {
                return GetSourceData(0, word_size, mod, reg, rm);
            }
        }

        // Generic function to save data to a destination based on the op code and address bytes
        protected void SaveToDestination(int data, int direction, int word_size, byte mod, byte reg, byte rm)
        {
            SaveToDestination(data, direction, word_size, mod, reg, rm, false);
        }
        protected void SaveToDestination(int data, int direction, int word_size, byte mod, byte reg, byte rm, bool useSREG)
        {
            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (useSREG)
                {
                    SaveWordToSegReg(reg, (ushort)data);
                }
                else
                {
                    EU.Registers.SaveRegisterValue(wordSize, reg, data);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            Bus.SaveData(useSREG ? 1 : word_size, GetRMTable1(rm), data);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            Bus.SaveData(useSREG ? 1 : word_size, GetRMTable2(mod, rm), data);
                            break;
                        }
                    case 0x03:
                        {
                            // If this instruction is using segmented registers then
                            // use a word size of 1 regardless of the instruction
                            EU.Registers.SaveRegisterValue( useSREG ? 1 : word_size, rm, data);
                            break;
                        }
                }
            }
        }
        #endregion

        #region R/M Tables
        // R/M Table 1 (mod=00)
        // returns the offset as a result of the operand
        protected int GetRMTable1(byte rm)
        {

            if(LastEACount == EU.InstructionCount)
            {
                return LastEAResult;
            }

            ushort result = 0;
            switch (rm)
            {
                case 0x00:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.SI);
                        EffectiveAddressClocks += 7;
                        break;
                    }
                case 0x01:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.DI);
                        EffectiveAddressClocks += 8;
                        break;
                    }
                case 0x02:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.SI);
                        EffectiveAddressClocks += 8;
                        break;
                    }
                case 0x03:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.DI);
                        EffectiveAddressClocks += 7;
                        break;
                    }
                case 0x04:
                    {
                        result = EU.Registers.SI;
                        EffectiveAddressClocks += 5;
                        break;
                    }
                case 0x05:
                    {
                        result = EU.Registers.DI;
                        EffectiveAddressClocks += 5;
                        break;
                    }
                case 0x06:
                    {
                        // direct address
                        EffectiveAddressClocks += 6;
                        result = GetImmediateWord();
                        break;
                    }
                case 0x07:
                    {
                        result = EU.Registers.BX;
                        EffectiveAddressClocks += 5;
                        break;
                    }
            }
            LastEAResult = result;
            LastEACount = EU.InstructionCount;
            return result;
        }

        // R/M Table 2 
        //      with 8 bit signed displacement (mod=01)
        //      with 16 bit unsigned displacement (mod=10)
        //
        // NOTE: rm=0x06 uses SS instead of DS as segment base
        // 
        // Returns the offset address as a result of the operand
        protected int GetRMTable2(byte mod, byte rm)
        {
            if (LastEACount == EU.InstructionCount)
            {
                return LastEAResult;
            }

            ushort result = 0;
            ushort disp = 0;
            switch (rm)
            {
                case 0x00:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.SI);
                        EffectiveAddressClocks += 7;
                        break;
                    }
                case 0x01:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.DI);
                        EffectiveAddressClocks += 8;
                        break;
                    }
                case 0x02:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.SI);
                        EffectiveAddressClocks += 8;
                        break;
                    }
                case 0x03:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.DI);
                        EffectiveAddressClocks += 7;
                        break;
                    }
                case 0x04:
                    {
                        result = EU.Registers.SI;
                        EffectiveAddressClocks += 5;
                        break;
                    }
                case 0x05:
                    {
                        result = EU.Registers.DI;
                        EffectiveAddressClocks += 5;
                        break;
                    }
                case 0x06:
                    {
                        Bus.UsingBasePointer = true;
                        result = EU.Registers.BP;
                        EffectiveAddressClocks += 5;
                        break;
                    }
                case 0x07:
                    {
                        result = EU.Registers.BX;
                        EffectiveAddressClocks += 5;
                        break;
                    }
            }
            switch (mod)
            {
                case 0x01:
                    {
                        // 8 bit displacement
                        disp = Bus.NextImmediate();
                        EffectiveAddressClocks += 4;
                        break;
                    }
                case 0x02:
                    {
                        // 16 bit displacement
                        disp = GetImmediateWord();
                        EffectiveAddressClocks += 4;
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", OpCode));
                    }
            }
            LastEAResult = (ushort)(result + disp);
            LastEACount = EU.InstructionCount;
            return LastEAResult;

        }
        #endregion

    }
}
