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

        private void InitOpCodeTable()
        {
            _opTable[0x88] = new OpCodeRecord(0x88, 10, ExecuteMOV_88);
            _opTable[0x89] = new OpCodeRecord(0x88, 10, ExecuteMOV_89);
            _opTable[0x8a] = new OpCodeRecord(0x88, 10, ExecuteMOV_90);
            _opTable[0x8b] = new OpCodeRecord(0x88, 10, ExecuteMOV_91);
        }


        private int ExecuteMOV_88()
        {
            // MOV Eb Gb
            //
            // d=0 (src=REG, dst=R/M)
            // w=0 (8 bit)

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            // mod determines the destination address or register
            switch (mod)
            {
                case 0x00:
                    {
                        _ ram[GetRMTable1(rm)] = GetRegField8(reg);
                        break;
                    }
                case 0x01:
                    {
                        break;
                    }
                case 0x02:
                    {
                        break;
                    }
                case 0x03:
                    {
                        break;
                    }
            }


            return 2;
        }

        private int ExecuteMOV_89()
        {
            return 2;
        }
        private int ExecuteMOV_90()
        {
            return 2;
        }
        private int ExecuteMOV_91()
        {
            return 2;
        }

        public void AdjustAfterAddition()
        {
            if( _reg.AL > 9 || _creg.AuxCarryFlag )
            {
                _reg.AL += 6;
                _reg.AH += 1;
                _creg.AuxCarryFlag = true;
                _creg.CarryFlag = true;
            }
            else
            {
                _creg.AuxCarryFlag = false;
                _creg.CarryFlag = false;
            }
            // always clear high nibble of AL
            _reg.AL = (byte)(_reg.AL & 0x0f);
        }

        #region "Operand Functions"

        #region "REG field operations"

        // Get 8 bit REG result (or R/M mod=11)
        private byte GetRegField8(byte reg)
        {
            byte result = 0;
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
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid REG value. reg={0:X2}", reg));
                    }
            }
            return result;
        }

        // Save 8 bit value in register indicated by REG
        private void SaveRegField8(byte reg, byte value)
        {
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
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid REG value. reg={0:X2}", reg));
                    }
            }

        }

        // Get 16 bit REG result (or R/M mod=11)
        private UInt16 GetRegField16(byte reg)
        {
            UInt16 result = 0;
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
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid REG value. reg={0:X2}", reg));
                    }
            }
            return result;
        }

        // Save 16 bit value in register indicated by REG
        private void SaveRegField16(byte reg, UInt16 value)
        {
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
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid REG value. reg={0:X2}", reg));
                    }
            }

        }

        #endregion  

        // Get 16 bit SREG result
        private UInt16 GetSegRegResult(byte reg)
        {
            UInt16 result = 0;
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
                case 0x06:
                    {
                        result = _bus.DS;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid reg value for SREG. reg={0:X2}", reg));
                    }
            }
            return result;
        }

        #region "R/M field operations"
        // R/M Table 1 (mod=00)
        // returns the physical address as a result of the operand
        private int GetRMTable1(byte rm)
        {
            UInt16 result = 0;
            UInt16 segment = _bus.DS;
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
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid RM Table1 value. rm={0:X2}", rm));
                    }
            }
            return Util.ConvertLogicalToPhysical(segment, result);
        }

        // R/M Table 2 
        //      with 8 bit signed displacement (mod=01)
        //      with 16 bit unsigned displacement (mod=10)
        //
        // NOTE: rm=0x06 uses SS instead of DS as segment base
        // 
        // Returns the physical address as a result of the operand
        private int GetRMTable2(byte mod, byte rm)
        {
            UInt16 result = 0;
            UInt16 disp = 0;
            UInt16 segment = _bus.DS;
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
                        segment = _bus.SS;
                        result = _reg.BP;
                        break;
                    }
                case 0x07:
                    {
                        result = _reg.BX;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(string.Format("Invalid RM Table2 value. rm={0:X2}", rm));
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
                        throw new InvalidOperationException(string.Format("Invalid RM Table2 mod value. mod={0:X2}", mod));
                    }
            }
            return Util.ConvertLogicalToPhysical(segment, (UInt16)(result+disp));

        }

        #endregion

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
    }
}
