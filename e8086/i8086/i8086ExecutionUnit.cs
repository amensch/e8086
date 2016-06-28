using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KDS.Utility;

namespace KDS.e8086
{
    public class i8086ExecutionUnit
    {
        /*

           EXECUTION UNIT (EU)                   BUS INTERFACE UNIT (BIU)
           --------------------                  ------------------------
            General Registers                      Segment Registers
                    |                             Instruction Pointer
                    |                                      |
                Operands                          Address Gen and Bus Control -----> external bus
                    |                                      |
                   ALU   <-----------------------  Instruction Queue
                    |  
                  Flags
        
           (From the 8086 Users Manual)
           The EU has no connection to the system bus -- the outside world.  It obtains instructions
           from a queue maintained by the BIU.  When an instruction requires access to memory or
           to a peripheral device, the EU requests the BIU to obtain or store the data.
           All addresses manipulated by the EU are 16 bits wide.  The BIU performs address relocation
           to give the EU access to the full megabyte of memory space.
           
           (From the 8086 Users Manual - paraphrased)
           The BIU performs all bus operations for the EU.   Data is transferred between the CPU
           and memory or I/O devices.

         */

        // Preserve the current OP code
        private byte _currentOP;

        // Table of OpCodes
        private OpCodeTable _opTable = new OpCodeTable();

        // General Registers: AX, BX, CX, DX and SP, BP, SI, DI
        private i8086Registers _reg = new i8086Registers();

        // Flags
        private i8086ConditionalRegister _creg = new i8086ConditionalRegister();

        // Bus Interface Unit
        private i8086BusInterfaceUnit _bus;

        public i8086ExecutionUnit(i8086BusInterfaceUnit bus)
        {
            _bus = bus;
            InitOpCodeTable();
        }

        public void NextInstruction()
        {
            _currentOP = _bus.NextIP();
            _opTable[_currentOP].opAction();
        }

        public i8086Registers Registers
        {
            get { return _reg; }
        }

        public i8086ConditionalRegister CondReg
        {
            get { return _creg; }
        }

        public i8086BusInterfaceUnit Bus
        {
            get { return _bus; }
        }

        private void InitOpCodeTable()
        {
            _opTable[0x00] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x01] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x02] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x03] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x04] = new OpCodeRecord(ExecuteADD_Immediate);
            _opTable[0x05] = new OpCodeRecord(ExecuteADD_Immediate);

            _opTable[0x88] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x89] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x8a] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x8b] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x8c] = new OpCodeRecord(ExecuteMOV_SReg);

            _opTable[0x8e] = new OpCodeRecord(ExecuteMOV_SReg);

            _opTable[0xa0] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa1] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa2] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa3] = new OpCodeRecord(ExecuteMOV_Mem);

            _opTable[0xb0] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb1] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb2] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb3] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb4] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb5] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb6] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb7] = new OpCodeRecord(ExecuteMOV_Imm8);
            _opTable[0xb8] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xb9] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xba] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xbb] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xbc] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xbd] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xbe] = new OpCodeRecord(ExecuteMOV_Imm16);
            _opTable[0xbf] = new OpCodeRecord(ExecuteMOV_Imm16);

            _opTable[0xc6] = new OpCodeRecord(ExecuteMOV_c6);
            _opTable[0xc7] = new OpCodeRecord(ExecuteMOV_c7);
        }

        private void ExecuteADD_General()
        {
            // Op Codes: 00-03
            // operand1 = operand1 + operand2
            // Flags: O S Z A P C

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = Util.GetDirection(_currentOP);
            int word_size = Util.GetWordSize(_currentOP);

            int source = GetSourceData(direction, word_size, mod, reg, rm);
            int result = ADD_Destination(source, direction, word_size, mod, reg, rm);
        }

        private void ExecuteADD_Immediate()
        {
            // Op Codes: 04-05
            // operand1 = operand1 + operand2
            // Flags: O S Z A P C

            int direction = 0;
            int word_size = Util.GetWordSize(_currentOP);
            int source;

            if (word_size == 0)
                source = _bus.NextIP();
            else
                source = GetImmediate16();

            // Set flags so the result gets stored in the proper accumulator (AL or AX)
            int result = ADD_Destination(source, direction, word_size, 0x03, 0x00, 0x00);
        }

        #region "MOV instructions"
        private void ExecuteMOV_General()
        {
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = Util.GetDirection(_currentOP);
            int word_size = Util.GetWordSize(_currentOP);

            int source = GetSourceData(direction, word_size, mod, reg, rm);
            SaveToDestination(source, direction, word_size, mod, reg, rm);
        }
        private void ExecuteMOV_SReg()
        {
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = Util.GetDirection(_currentOP);
            int word_size = Util.GetWordSize(_currentOP);

            int source = GetSourceData(direction, word_size, mod, reg, rm, true);
            SaveToDestination(source, direction, word_size, mod, reg, rm, true);
        }
        private void ExecuteMOV_Mem()
        {
            // MOV MEM <-> IMM (8 and 16)
            byte lo = Bus.NextIP();
            byte hi = Bus.NextIP();
            switch(_currentOP)
            {
                case 0xa0:
                    {
                        _reg.AL = _bus.GetData8(Util.GetValue16(hi, lo));
                        break;
                    }
                case 0xa1:
                    {
                        _reg.AX = _bus.GetData16(Util.GetValue16(hi, lo));
                        break;
                    }
                case 0xa2:
                    {
                        _bus.SaveData8(Util.GetValue16(hi, lo), _reg.AL);
                        break;
                    }
                case 0xa3:
                    {
                        _bus.SaveData16(Util.GetValue16(hi, lo), _reg.AX);
                        break;
                    }
            }
        }
        private void ExecuteMOV_Imm8()
        {
            // Covers op codes B0-B7
            // MOV <reg>, IMM-8
            byte mem = _bus.NextIP();
            switch(_currentOP)
            {
                case 0xb0:
                    {
                        _reg.AL = mem;
                        break;
                    }
                case 0xb1:
                    {
                        _reg.CL = mem;
                        break;
                    }
                case 0xb2:
                    {
                        _reg.DL = mem;
                        break;
                    }
                case 0xb3:
                    {
                        _reg.BL = mem;
                        break;
                    }
                case 0xb4:
                    {
                        _reg.AH = mem;
                        break;
                    }
                case 0xb5:
                    {
                        _reg.CH = mem;
                        break;
                    }
                case 0xb6:
                    {
                        _reg.DH = mem;
                        break;
                    }
                case 0xb7:
                    {
                        _reg.BH = mem;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(string.Format("You should not be here! op={0:X2}", _currentOP));
                    }
            }
        }
        private void ExecuteMOV_Imm16()
        {
            // Covers op codes B8-BF
            // MOV <reg>, IMM-16

            switch (_currentOP)
            {
                case 0xb8:
                    {
                        _reg.AX = GetImmediate16();
                        break;
                    }
                case 0xb9:
                    {
                        _reg.CX = GetImmediate16();
                        break;
                    }
                case 0xba:
                    {
                        _reg.DX = GetImmediate16();
                        break;
                    }
                case 0xbb:
                    {
                        _reg.BX = GetImmediate16();
                        break;
                    }
                case 0xbc:
                    {
                        _reg.SP = GetImmediate16();
                        break;
                    }
                case 0xbd:
                    {
                        _reg.BP = GetImmediate16();
                        break;
                    }
                case 0xbe:
                    {
                        _reg.SI = GetImmediate16();
                        break;
                    }
                case 0xbf:
                    {
                        _reg.DI = GetImmediate16();
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(string.Format("You should not be here! op={0:X2}", _currentOP));
                    }
            }
        }
        private void ExecuteMOV_c6()
        {
            // MOV MEM-8, IMM-8
            // displacement bytes are optional so don't retrieve the immediate value
            // until the destination offset has been determined.
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);
            
            int dest;
            AssertMOD(mod);
            switch (mod)
            {
                case 0x00:
                    {
                        dest = GetRMTable1(rm);
                        _bus.SaveData8(dest, _bus.NextIP());
                        break;
                    }
                case 0x01:
                    {
                        dest = GetRMTable2(mod, rm);
                        _bus.SaveData8(dest, _bus.NextIP());
                        break;
                    }
                case 0x02:
                    {
                        dest = GetRMTable2(mod, rm);
                        _bus.SaveData8(dest, _bus.NextIP());
                        break;
                    }
                case 0x03:
                    {
                        SaveRegField8(rm, _bus.NextIP());
                        break;
                    }
            }
        }
        private void ExecuteMOV_c7()
        {
            // MOV MEM-16, IMM-16
            // displacement bytes are optional so don't retrieve the immediate value
            // until the destination offset has been determined.
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int dest;
            AssertMOD(mod);
            switch (mod)
            {
                case 0x00:
                    {
                        dest = GetRMTable1(rm);
                        _bus.SaveData16(dest, GetImmediate16());
                        break;
                    }
                case 0x01:
                    {
                        dest = GetRMTable2(mod, rm);
                        _bus.SaveData16(dest, GetImmediate16());
                        break;
                    }
                case 0x02:
                    {
                        dest = GetRMTable2(mod, rm);
                        _bus.SaveData16(dest, GetImmediate16());
                        break;
                    }
                case 0x03:
                    {
                        SaveRegField16(rm, GetImmediate16());
                        break;
                    }
            }
        }
        #endregion

        #region "Generic Retrieve and Store functions"
        // Generic function to retrieve source data based on the op code and addr byte
        private int GetSourceData(int direction, int word_size, byte mod, byte reg, byte rm)
        {
            return GetSourceData(direction, word_size, mod, reg, rm, false);
        }
        private int GetSourceData(int direction, int word_size, byte mod, byte reg, byte rm, bool useSREG)
        {
            int result = 0;

            // Action is the same if the direction is 0 (source is REG)
            if (direction == 0)
            {
                if (useSREG)
                {
                    result = GetSegRegField(reg);
                }
                else
                {
                    if (word_size == 0)
                    {
                        result = GetRegField8(reg);
                    }
                    else
                    {
                        result = GetRegField16(reg);
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
                                result = _bus.GetData8(GetRMTable1(rm));
                            }
                            else //if ((direction == 1) && (word_size == 1))
                            {
                                result = _bus.GetData16(GetRMTable1(rm));
                            }
                            break;
                        }
                    case 0x01:
                    case 0x02:   // difference is processed in the GetRMTable2 function
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                result = _bus.GetData8(GetRMTable2(mod, rm));
                            }
                            else //if ((direction == 1) && (word_size == 1))
                            {
                                result = _bus.GetData16(GetRMTable2(mod, rm));
                            }
                            break;
                        }
                    case 0x03:
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                result = GetRegField8(rm);
                            }
                            else //if ((direction == 1) && (word_size == 1))
                            {
                                result = GetRegField16(rm);
                            }
                            break;
                        }
                }
            }
            return result;
        }

        // Generic function to retrieve the current data in the destination based on the op code and addr byte
        private int GetDestinationData(int direction, int word_size, byte mod, byte reg, byte rm)
        {
            if(direction == 0)
            {
                return GetSourceData(1, word_size, mod, reg, rm);
            }
            else
            {
                return GetSourceData(0, word_size, mod, reg, rm);
            }
        }

        // Generic function to save data to a destination based on the op code and address bytes
        private void SaveToDestination(int data, int direction, int word_size, byte mod, byte reg, byte rm)
        {
            SaveToDestination(data, direction, word_size, mod, reg, rm, false);
        }
        private void SaveToDestination(int data, int direction, int word_size, byte mod, byte reg, byte rm, bool useSREG)
        {
            AssertMOD(mod);

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (useSREG)
                {
                    SaveSegRegField(reg, (UInt16)data);
                }
                else
                {
                    if (word_size == 0)
                    {
                        SaveRegField8(reg, (byte)data);
                    }
                    else
                    {
                        SaveRegField16(reg, (UInt16)data);
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
                                _bus.SaveData8(GetRMTable1(rm), (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                _bus.SaveData16(GetRMTable1(rm), (UInt16)data);
                            }
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                _bus.SaveData8(GetRMTable2(mod, rm), (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                _bus.SaveData16(GetRMTable2(mod, rm), (UInt16)data);
                            }
                            break;
                        }
                    case 0x03:
                        {
                            if ((word_size == 0) && !useSREG)
                            {
                                SaveRegField8(rm, (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                SaveRegField16(rm, (UInt16)data);
                            }
                            break;
                        }
                }
            }
        }
        #endregion

        // returns the result
        private int ADD_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm)
        {
            AssertMOD(mod);
            int result = 0;
            int offset;
            int dest = 0;

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (word_size == 0)
                {
                    dest = GetRegField8(reg);
                    result = source + dest;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = source + dest;
                    SaveRegField16(reg, (UInt16)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = _bus.GetData(word_size, offset);
                            result = dest + source;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = _bus.GetData(word_size, offset);
                            result = dest + source;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0) 
                            {
                                dest = GetRegField8(rm);
                                result = source + dest;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = source + dest;
                                SaveRegField16(rm, (UInt16)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            _creg.CalcOverflowFlag(word_size, source, dest);
            _creg.CalcSignFlag(word_size, result);
            _creg.CalcZeroFlag(word_size, result);
            _creg.CalcAuxCarryFlag(source, dest);
            _creg.CalcParityFlag(result);
            _creg.CalcCarryFlag(word_size, result);
            return result;
        }


        #region "Get and Set registers"
        // Get 8 bit REG result (or R/M mod=11)
        private byte GetRegField8(byte reg)
        {
            byte result = 0;
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        result = _reg.AL;
                        break;
                    }
                case 0x01:
                    {
                        result = _reg.CL;
                        break;
                    }
                case 0x02:
                    {
                        result = _reg.DL;
                        break;
                    }
                case 0x03:
                    {
                        result = _reg.BL;
                        break;
                    }
                case 0x04:
                    {
                        result = _reg.AH;
                        break;
                    }
                case 0x05:
                    {
                        result = _reg.CH;
                        break;
                    }
                case 0x06:
                    {
                        result = _reg.DH;
                        break;
                    }
                case 0x07:
                    {
                        result = _reg.BH;
                        break;
                    }
            }
            return result;
        }

        // Save 8 bit value in register indicated by REG
        private void SaveRegField8(byte reg, byte value)
        {
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                       _reg.AL = value;
                        break;
                    }
                case 0x01:
                    {
                        _reg.CL = value;
                        break;
                    }
                case 0x02:
                    {
                        _reg.DL = value;
                        break;
                    }
                case 0x03:
                    {
                        _reg.BL = value;
                        break;
                    }
                case 0x04:
                    {
                        _reg.AH = value;
                        break;
                    }
                case 0x05:
                    {
                        _reg.CH = value;
                        break;
                    }
                case 0x06:
                    {
                        _reg.DH = value;
                        break;
                    }
                case 0x07:
                    {
                        _reg.BH = value;
                        break;
                    }
            }

        }

        // Get 16 bit REG result (or R/M mod=11)
        private UInt16 GetRegField16(byte reg)
        {
            UInt16 result = 0;
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        result = _reg.AX;
                        break;
                    }
                case 0x01:
                    {
                        result = _reg.CX;
                        break;
                    }
                case 0x02:
                    {
                        result = _reg.DX;
                        break;
                    }
                case 0x03:
                    {
                        result = _reg.BX;
                        break;
                    }
                case 0x04:
                    {
                        result = _reg.SP;
                        break;
                    }
                case 0x05:
                    {
                        result = _reg.BP;
                        break;
                    }
                case 0x06:
                    {
                        result = _reg.SI;
                        break;
                    }
                case 0x07:
                    {
                        result = _reg.DI;
                        break;
                    }
            }
            return result;
        }

        // Save 16 bit value in register indicated by REG
        private void SaveRegField16(byte reg, UInt16 value)
        {
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        _reg.AX = value;
                        break;
                    }
                case 0x01:
                    {
                        _reg.CX = value;
                        break;
                    }
                case 0x02:
                    {
                        _reg.DX = value;
                        break;
                    }
                case 0x03:
                    {
                        _reg.BX = value;
                        break;
                    }
                case 0x04:
                    {
                        _reg.SP = value;
                        break;
                    }
                case 0x05:
                    {
                        _reg.BP = value;
                        break;
                    }
                case 0x06:
                    {
                        _reg.SI = value;
                        break;
                    }
                case 0x07:
                    {
                        _reg.DI = value;
                        break;
                    }
            }

        }

        // Get 16 bit SREG result
        private UInt16 GetSegRegField(byte reg)
        {
            UInt16 result = 0;
            AssertSREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        result = _bus.ES;
                        break;
                    }
                case 0x01:
                    {
                        result = _bus.CS;
                        break;
                    }
                case 0x02:
                    {
                        result = _bus.SS;
                        break;
                    }
                case 0x03:
                    {
                        result = _bus.DS;
                        break;
                    }
            }
            return result;
        }

        // Save 16 bit value into a Seg Reg
        private void SaveSegRegField(byte reg, UInt16 value)
        {
            AssertSREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        _bus.ES = value;
                        break;
                    }
                case 0x01:
                    {
                        _bus.CS = value;
                        break;
                    }
                case 0x02:
                    {
                        _bus.SS = value;
                        break;
                    }
                case 0x06:
                    {
                        _bus.DS = value;
                        break;
                    }
            }
        }
        #endregion

        #region "R/M Tables"
        // R/M Table 1 (mod=00)
        // returns the offset as a result of the operand
        private int GetRMTable1(byte rm)
        {
            UInt16 result = 0;
            AssertRM(rm);
            switch (rm)
            {
                case 0x00:
                    {
                        result = (UInt16)(_reg.BX + _reg.SI);
                        break;
                    }
                case 0x01:
                    {
                        result = (UInt16)(_reg.BX + _reg.DI);
                        break;
                    }
                case 0x02:
                    {
                        result = (UInt16)(_reg.BP + _reg.SI);
                        break;
                    }
                case 0x03:
                    {
                        result = (UInt16)(_reg.BP + _reg.DI);
                        break;
                    }
                case 0x04:
                    {
                        result = _reg.SI;
                        break;
                    }
                case 0x05:
                    {
                        result = _reg.DI;
                        break;
                    }
                case 0x06:
                    {
                        // direct address
                        result = GetImmediate16();
                        break;
                    }
                case 0x07:
                    {
                        result = _reg.BX;
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
        private int GetRMTable2(byte mod, byte rm)
        {
            UInt16 result = 0;
            UInt16 disp = 0;
            AssertRM(rm);
            switch (rm)
            {
                case 0x00:
                    {
                        result = (UInt16)(_reg.BX + _reg.SI);
                        break;
                    }
                case 0x01:
                    {
                        result = (UInt16)(_reg.BX + _reg.DI);
                        break;
                    }
                case 0x02:
                    {
                        result = (UInt16)(_reg.BP + _reg.SI);
                        break;
                    }
                case 0x03:
                    {
                        result = (UInt16)(_reg.BP + _reg.DI);
                        break;
                    }
                case 0x04:
                    {
                        result = _reg.SI;
                        break;
                    }
                case 0x05:
                    {
                        result = _reg.DI;
                        break;
                    }
                case 0x06:
                    {
                        _bus.UsingBasePointer = true;
                        result = _reg.BP;
                        break;
                    }
                case 0x07:
                    {
                        result = _reg.BX;
                        break;
                    }
            }
            switch(mod)
            {
                case 0x01:
                    {
                        // 8 bit displacement
                        disp = _bus.NextIP();
                        break;
                    }
                case 0x02:
                    {
                        // 16 bit displacement
                        disp = GetImmediate16();
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", _currentOP));
                    }
            }
            return (UInt16)(result + disp);

        }
        #endregion  

        // Gets the immediate 16 bit value
        private UInt16 GetImmediate16()
        {
            byte lo = _bus.NextIP();
            byte hi = _bus.NextIP();
            return Util.GetValue16(hi, lo);
        }

        private void SplitAddrByte(byte addr, ref byte mod, ref byte reg, ref byte rm)
        {
            mod = Util.GetMODValue(addr);
            reg = Util.GetREGValue(addr);
            rm = Util.GetRMValue(addr);
        }

        // Assert a proper mod value
        private void AssertMOD(byte mod)
        {
            if( mod > 0x03 )
            {
                throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", _currentOP));
            }
        }

        // Assert a proper reg value
        private void AssertREG(byte reg)
        {
            if (reg > 0x07)
            {
                throw new ArgumentOutOfRangeException("reg", reg, string.Format("Invalid reg value in opcode={0:X2}", _currentOP));
            }
        }

        // Assert a proper sreg value
        private void AssertSREG(byte reg)
        {
            if (reg > 0x03)
            {
                throw new ArgumentOutOfRangeException("reg", reg, string.Format("Invalid sreg value in opcode={0:X2}", _currentOP));
            }
        }
        
        // Assert a proper rm value
        private void AssertRM(byte rm)
        {
            if (rm > 0x07)
            {
                throw new ArgumentOutOfRangeException("rm", rm, string.Format("Invalid rm value in opcode={0:X2}", _currentOP));
            }
        }
    }
}
