using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public abstract class Instruction
    {
        public byte OpCode { get; private set; }
        protected AddressMode OpCodeMode { get; private set; }

        protected IExecutionUnit EU { get; private set; }
        protected IBus Bus { get; private set; }
        protected int direction;
        protected int wordSize;

        private long LastLookupCount = 0;
        private ushort LastLookupValue = 0;

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
            PreProcessing();
            ExecuteInstruction();
        }

        #region Helper and Utility functions

        // Gets the immediate 16 bit value
        protected ushort GetImmediateWord()
        {
            byte lo = Bus.NextIP();
            byte hi = Bus.NextIP();
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

        // Assert a proper mod value
        protected void AssertMOD(byte mod)
        {
            if (mod > 0x03)
            {
                throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", OpCode));
            }
        }

        // Assert a proper reg value
        protected void AssertREG(byte reg)
        {
            if (reg > 0x07)
            {
                throw new ArgumentOutOfRangeException("reg", reg, string.Format("Invalid reg value in opcode={0:X2}", OpCode));
            }
        }

        // Assert a proper sreg value
        protected void AssertSREG(byte reg)
        {
            if (reg > 0x03)
            {
                throw new ArgumentOutOfRangeException("reg", reg, string.Format("Invalid sreg value in opcode={0:X2}", OpCode));
            }
        }

        // Assert a proper rm value
        protected void AssertRM(byte rm)
        {
            if (rm > 0x07)
            {
                throw new ArgumentOutOfRangeException("rm", rm, string.Format("Invalid rm value in opcode={0:X2}", OpCode));
            }
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

        #region Get and Set registers
        // Get 8 bit REG result (or R/M mod=11)
        protected byte GetByteFromRegisters(byte reg)
        {
            byte result = 0;
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        result = EU.Registers.AL;
                        break;
                    }
                case 0x01:
                    {
                        result = EU.Registers.CL;
                        break;
                    }
                case 0x02:
                    {
                        result = EU.Registers.DL;
                        break;
                    }
                case 0x03:
                    {
                        result = EU.Registers.BL;
                        break;
                    }
                case 0x04:
                    {
                        result = EU.Registers.AH;
                        break;
                    }
                case 0x05:
                    {
                        result = EU.Registers.CH;
                        break;
                    }
                case 0x06:
                    {
                        result = EU.Registers.DH;
                        break;
                    }
                case 0x07:
                    {
                        result = EU.Registers.BH;
                        break;
                    }
            }
            return result;
        }

        // Save 8 bit value in register indicated by REG
        protected void SaveByteToRegisters(byte reg, byte value)
        {
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        EU.Registers.AL = value;
                        break;
                    }
                case 0x01:
                    {
                        EU.Registers.CL = value;
                        break;
                    }
                case 0x02:
                    {
                        EU.Registers.DL = value;
                        break;
                    }
                case 0x03:
                    {
                        EU.Registers.BL = value;
                        break;
                    }
                case 0x04:
                    {
                        EU.Registers.AH = value;
                        break;
                    }
                case 0x05:
                    {
                        EU.Registers.CH = value;
                        break;
                    }
                case 0x06:
                    {
                        EU.Registers.DH = value;
                        break;
                    }
                case 0x07:
                    {
                        EU.Registers.BH = value;
                        break;
                    }
            }

        }

        // Get 16 bit REG result (or R/M mod=11)
        protected ushort GetWordFromRegisters(byte reg)
        {
            ushort result = 0;
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        result = EU.Registers.AX;
                        break;
                    }
                case 0x01:
                    {
                        result = EU.Registers.CX;
                        break;
                    }
                case 0x02:
                    {
                        result = EU.Registers.DX;
                        break;
                    }
                case 0x03:
                    {
                        result = EU.Registers.BX;
                        break;
                    }
                case 0x04:
                    {
                        result = EU.Registers.SP;
                        break;
                    }
                case 0x05:
                    {
                        result = EU.Registers.BP;
                        break;
                    }
                case 0x06:
                    {
                        result = EU.Registers.SI;
                        break;
                    }
                case 0x07:
                    {
                        result = EU.Registers.DI;
                        break;
                    }
            }
            return result;
        }

        // Save 16 bit value in register indicated by REG
        protected void SaveWordToRegisters(byte reg, ushort value)
        {
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        EU.Registers.AX = value;
                        break;
                    }
                case 0x01:
                    {
                        EU.Registers.CX = value;
                        break;
                    }
                case 0x02:
                    {
                        EU.Registers.DX = value;
                        break;
                    }
                case 0x03:
                    {
                        EU.Registers.BX = value;
                        break;
                    }
                case 0x04:
                    {
                        EU.Registers.SP = value;
                        break;
                    }
                case 0x05:
                    {
                        EU.Registers.BP = value;
                        break;
                    }
                case 0x06:
                    {
                        EU.Registers.SI = value;
                        break;
                    }
                case 0x07:
                    {
                        EU.Registers.DI = value;
                        break;
                    }
            }

        }

        // Get 16 bit SREG result
        protected ushort GetWordFromSegReg(byte reg)
        {
            ushort result = 0;
            AssertSREG(reg);
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
            AssertSREG(reg);
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
                    if (word_size == 0)
                    {
                        result = GetByteFromRegisters(reg);
                    }
                    else
                    {
                        result = GetWordFromRegisters(reg);
                    }
                }
            }
            else
            {
                AssertMOD(mod);
                switch (mod)
                {
                    case 0x00:
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                result = Bus.GetByte(GetRMTable1(rm));
                            }
                            else //if ((direction == 1) && (word_size == 1))
                            {
                                result = Bus.GetWord(GetRMTable1(rm));
                            }
                            break;
                        }
                    case 0x01:
                    case 0x02:   // difference is processed in the GetRMTable2 function
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                result = Bus.GetByte(GetRMTable2(mod, rm));
                            }
                            else //if ((direction == 1) && (word_size == 1))
                            {
                                result = Bus.GetWord(GetRMTable2(mod, rm));
                            }
                            break;
                        }
                    case 0x03:
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                result = GetByteFromRegisters(rm);
                            }
                            else //if ((direction == 1) && (word_size == 1))
                            {
                                result = GetWordFromRegisters(rm);
                            }
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
            AssertMOD(mod);

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (useSREG)
                {
                    SaveWordToSegReg(reg, (ushort)data);
                }
                else
                {
                    if (word_size == 0)
                    {
                        SaveByteToRegisters(reg, (byte)data);
                    }
                    else
                    {
                        SaveWordToRegisters(reg, (ushort)data);
                    }
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                Bus.SaveByte(GetRMTable1(rm), (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                Bus.SaveWord(GetRMTable1(rm), (ushort)data);
                            }
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                Bus.SaveByte(GetRMTable2(mod, rm), (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                Bus.SaveWord(GetRMTable2(mod, rm), (ushort)data);
                            }
                            break;
                        }
                    case 0x03:
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                SaveByteToRegisters(rm, (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                SaveWordToRegisters(rm, (ushort)data);
                            }
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
            ushort result = 0;
            AssertRM(rm);
            switch (rm)
            {
                case 0x00:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.SI);
                        break;
                    }
                case 0x01:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.DI);
                        break;
                    }
                case 0x02:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.SI);
                        break;
                    }
                case 0x03:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.DI);
                        break;
                    }
                case 0x04:
                    {
                        result = EU.Registers.SI;
                        break;
                    }
                case 0x05:
                    {
                        result = EU.Registers.DI;
                        break;
                    }
                case 0x06:
                    {
                        // direct address
                        // There is only one RM table lookup per instruction but this may be invoked more than once
                        // for certain instructions.  For this reason we preserve the offset in case it is needed again.
                        if (LastLookupCount == EU.InstructionCount)
                        {
                            result = LastLookupValue;
                        }
                        else
                        {
                            result = GetImmediateWord();
                            LastLookupValue = result;
                            LastLookupCount = EU.InstructionCount;

                        }
                        break;
                    }
                case 0x07:
                    {
                        result = EU.Registers.BX;
                        break;
                    }
            }
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
            ushort result = 0;
            ushort disp = 0;
            AssertRM(rm);
            switch (rm)
            {
                case 0x00:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.SI);
                        break;
                    }
                case 0x01:
                    {
                        result = (ushort)(EU.Registers.BX + EU.Registers.DI);
                        break;
                    }
                case 0x02:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.SI);
                        break;
                    }
                case 0x03:
                    {
                        result = (ushort)(EU.Registers.BP + EU.Registers.DI);
                        break;
                    }
                case 0x04:
                    {
                        result = EU.Registers.SI;
                        break;
                    }
                case 0x05:
                    {
                        result = EU.Registers.DI;
                        break;
                    }
                case 0x06:
                    {
                        Bus.UsingBasePointer = true;
                        result = EU.Registers.BP;
                        break;
                    }
                case 0x07:
                    {
                        result = EU.Registers.BX;
                        break;
                    }
            }
            switch (mod)
            {
                // There is only one RM table lookup per instruction but this may be invoked more than once
                // for certain instructions.  For this reason we preserve the offset in case it is needed again.
                case 0x01:
                    {
                        // 8 bit displacement
                        if (LastLookupCount == EU.InstructionCount)
                        {
                            disp = LastLookupValue;
                        }
                        else
                        {
                            disp = Bus.NextIP();
                            LastLookupValue = disp;
                            LastLookupCount = EU.InstructionCount;
                        }
                        break;
                    }
                case 0x02:
                    {
                        // 16 bit displacement
                        if (LastLookupCount == EU.InstructionCount)
                        {
                            disp = LastLookupValue;
                        }
                        else
                        {
                            disp = GetImmediateWord();
                            LastLookupValue = disp;
                            LastLookupCount = EU.InstructionCount;
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", OpCode));
                    }
            }
            return (ushort)(result + disp);

        }
        #endregion


    }
}
