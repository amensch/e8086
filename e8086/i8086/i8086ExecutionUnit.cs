using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Statistics
        private Statistics _stats = new Statistics();
        private long _instrCount = 0;
        private long _RMTableLookupCount = 0;
        private UInt16 _RMTableLastLookup = 0;

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

        // I/O Ports
        private Dictionary<UInt16, IInputDevice> _inputDevices;
        private Dictionary<UInt16, IOutputDevice> _outputDevices;

        // Repeat flag
        private bool _repeat = false;
        private int _repeatType = 0;

        // Property to indicate a halt has been encountered
        public bool Halted { get; set; }

        public i8086ExecutionUnit(i8086BusInterfaceUnit bus)
        {
            _bus = bus;
            Halted = false;

            _inputDevices = new Dictionary<UInt16, IInputDevice>();
            _outputDevices = new Dictionary<UInt16, IOutputDevice>();

            InitOpCodeTable();
        }

        // Internal components are exposed for debugging and testing purposes

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

        public Statistics Stats
        {
            get { return _stats; }
        }

        public void NextInstruction()
        {
            _instrCount++;
            _currentOP = _bus.NextIP();

            // count statistics
            _stats.AddOpCode(_currentOP);

            // if segment override process that right here
            if (_currentOP == 0x26)
            {
                _bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseES;
            }
            else if (_currentOP == 0x2e)
            {
                _bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseCS;
            }
            else if (_currentOP == 0x36)
            {
                _bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseSS;
            }
            else if (_currentOP == 0x3e)
            {
                _bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.UseDS;
            }
            else
            {
                _opTable[_currentOP].opAction();

                // after executing the instruction reset the override and base pointer flags
                _bus.SegmentOverride = i8086BusInterfaceUnit.SegmentOverrideState.NoOverride;
                _bus.UsingBasePointer = false;
            }
        }

        public void AddInputDevice(IInputDevice device, byte port)
        {
            if (_inputDevices.ContainsKey(port))
            {
                _inputDevices.Remove(port);
            }
            _inputDevices.Add(port, device);
        }

        public void AddOutputDevice(IOutputDevice device, byte port)
        {
            if (_outputDevices.ContainsKey(port))
            {
                _outputDevices.Remove(port);
            }
            _outputDevices.Add(port, device);
        }

        private void InitOpCodeTable()
        {
            _opTable[0x00] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x01] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x02] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x03] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x04] = new OpCodeRecord(ExecuteADD_Immediate);
            _opTable[0x05] = new OpCodeRecord(ExecuteADD_Immediate);
            _opTable[0x06] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x07] = new OpCodeRecord(Execute_POP);
            _opTable[0x08] = new OpCodeRecord(ExecuteLogical_General);      // OR 
            _opTable[0x09] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x0a] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x0b] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x0c] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0x0d] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0x0e] = new OpCodeRecord(Execute_PUSH);
            // POP CS is not a valid instruction
            _opTable[0x0f] = new OpCodeRecord(() => { });
            _opTable[0x10] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x11] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x12] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x13] = new OpCodeRecord(ExecuteADD_General);
            _opTable[0x14] = new OpCodeRecord(ExecuteADD_Immediate);
            _opTable[0x15] = new OpCodeRecord(ExecuteADD_Immediate);
            _opTable[0x16] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x17] = new OpCodeRecord(Execute_POP);
            _opTable[0x18] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x19] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x1a] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x1b] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x1c] = new OpCodeRecord(ExecuteSUB_Immediate);
            _opTable[0x1d] = new OpCodeRecord(ExecuteSUB_Immediate);
            _opTable[0x1e] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x1f] = new OpCodeRecord(Execute_POP);  
            _opTable[0x20] = new OpCodeRecord(ExecuteLogical_General);     // AND
            _opTable[0x21] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x22] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x23] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x24] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0x25] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x26]  segment override is processed in the NextInstruction() method
            _opTable[0x27] = new OpCodeRecord(Execute_DecimalAdjustADD);
            _opTable[0x28] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x29] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x2a] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x2b] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x2c] = new OpCodeRecord(ExecuteSUB_Immediate);
            _opTable[0x2d] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x2e]  segment override is processed in the NextInstruction() method
            _opTable[0x2f] = new OpCodeRecord(Execute_DecimalAdjustSUB);
            _opTable[0x30] = new OpCodeRecord(ExecuteLogical_General);  // XOR
            _opTable[0x31] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x32] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x33] = new OpCodeRecord(ExecuteLogical_General);
            _opTable[0x34] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0x35] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x36]  segment override is processed in the NextInstruction() method
            _opTable[0x37] = new OpCodeRecord(Execute_AsciiAdjustADD);
            _opTable[0x38] = new OpCodeRecord(ExecuteSUB_General);       // CMP 
            _opTable[0x39] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x3a] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x3b] = new OpCodeRecord(ExecuteSUB_General);
            _opTable[0x3c] = new OpCodeRecord(ExecuteSUB_Immediate);
            _opTable[0x3d] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x3e]  segment override is processed in the NextInstruction() method
            _opTable[0x3f] = new OpCodeRecord(Execute_AsciiAdjustSUB);
            _opTable[0x40] = new OpCodeRecord(ExecuteINC);
            _opTable[0x41] = new OpCodeRecord(ExecuteINC);
            _opTable[0x42] = new OpCodeRecord(ExecuteINC);
            _opTable[0x43] = new OpCodeRecord(ExecuteINC);
            _opTable[0x44] = new OpCodeRecord(ExecuteINC);
            _opTable[0x45] = new OpCodeRecord(ExecuteINC);
            _opTable[0x46] = new OpCodeRecord(ExecuteINC);
            _opTable[0x47] = new OpCodeRecord(ExecuteINC);
            _opTable[0x48] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x49] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x4a] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x4b] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x4c] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x4d] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x4e] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x4f] = new OpCodeRecord(ExecuteDEC);
            _opTable[0x50] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x51] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x52] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x53] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x54] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x55] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x56] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x57] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x58] = new OpCodeRecord(Execute_POP);
            _opTable[0x59] = new OpCodeRecord(Execute_POP);
            _opTable[0x5a] = new OpCodeRecord(Execute_POP);
            _opTable[0x5b] = new OpCodeRecord(Execute_POP);
            _opTable[0x5c] = new OpCodeRecord(Execute_POP);
            _opTable[0x5d] = new OpCodeRecord(Execute_POP);
            _opTable[0x5e] = new OpCodeRecord(Execute_POP);
            _opTable[0x5f] = new OpCodeRecord(Execute_POP);
            _opTable[0x60] = new OpCodeRecord(Execute_PUSHA);
            _opTable[0x61] = new OpCodeRecord(Execute_POPA);
            _opTable[0x62] = new OpCodeRecord(Execute_Bound);
            _opTable[0x63] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x63 is not implemented"); });
            _opTable[0x64] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x64 is not implemented"); });
            _opTable[0x65] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x65 is not implemented"); });
            _opTable[0x66] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x66 is not implemented"); });
            _opTable[0x67] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x67 is not implemented"); });
            _opTable[0x68] = new OpCodeRecord(() =>  // PUSH imm-16
            {
                Push(GetImmediate16());
            });
            _opTable[0x69] = new OpCodeRecord(() => // IMUL REG-16, RM-16, Imm
            {
                byte mod = 0, reg = 0, rm = 0;
                SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

                int word_size = GetWordSize();
                int direction = GetDirection();

                UInt16 oper1 = (UInt16)GetSourceData(direction, word_size, mod, reg, rm);
                UInt16 oper2 = GetImmediate16();

                UInt32 oper1ext = SignExtend32(oper1);
                UInt32 oper2ext = SignExtend32(oper2);

                UInt32 result = oper1ext * oper2ext;

                SaveToDestination((UInt16)(result & 0xffff), direction, word_size, mod, reg, rm);

                if( (result & 0xffff0000) != 0 )
                {
                    _creg.CarryFlag = true;
                    _creg.OverflowFlag = true;
                }
                else
                {
                    _creg.CarryFlag = false;
                    _creg.OverflowFlag = false;
                }

            });
            _opTable[0x6a] = new OpCodeRecord(() =>  // PUSH imm-8
            {
                Push(_bus.NextIP());
            });
            _opTable[0x6b] = new OpCodeRecord(() => 
            {
                byte mod = 0, reg = 0, rm = 0;
                SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

                int word_size = GetWordSize();
                int direction = GetDirection();

                UInt16 oper1 = (UInt16)GetSourceData(direction, word_size, mod, reg, rm);
                UInt16 oper2 = _bus.NextIP();

                UInt32 oper1ext = SignExtend32(oper1);
                UInt32 oper2ext = SignExtend32(oper2);

                UInt32 result = oper1ext * oper2ext;

                SaveToDestination((UInt16)(result & 0xffff), direction, word_size, mod, reg, rm);

                if ((result & 0xffff0000) != 0)
                {
                    _creg.CarryFlag = true;
                    _creg.OverflowFlag = true;
                }
                else
                {
                    _creg.CarryFlag = false;
                    _creg.OverflowFlag = false;
                }
            });

            _opTable[0x6c] = new OpCodeRecord(() => // INSB
            {
                // if repetition is on but CX is 0 then do nothing
                if( ( _repeat ) && ( _reg.CX == 0 ) )
                {
                }
                else
                {
                    do
                    {
                        Execute_IN_String();

                        if (_creg.DirectionFlag)
                            _reg.DI--;
                        else
                            _reg.DI++;

                        if (_repeat)
                            _reg.CX--;
                    } while (_reg.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x6d] = new OpCodeRecord(() => // INSW
            {
                // if repetition is on but CX is 0 then do nothing
                if ((_repeat) && (_reg.CX == 0))
                {
                }
                else
                {
                    do
                    {
                        Execute_IN_String();

                            if (_creg.DirectionFlag)
                                _reg.DI -= 2;
                            else
                                _reg.DI += 2;

                        if (_repeat)
                            _reg.CX--;
                    } while (_reg.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x6e] = new OpCodeRecord(() => // OUTSB
            {
                // if repetition is on but CX is 0 then do nothing
                if ((_repeat) && (_reg.CX == 0))
                {
                }
                else
                {
                    do
                    {
                        Execute_OUT_String();

                        if (_creg.DirectionFlag)
                            _reg.SI -= 1;
                        else
                            _reg.SI += 1;

                        if (_repeat)
                            _reg.CX--;
                    } while (_reg.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x6f] = new OpCodeRecord(() => // OUTSW
            {
                // if repetition is on but CX is 0 then do nothing
                if ((_repeat) && (_reg.CX == 0))
                {
                }
                else
                {
                    do
                    {
                        Execute_OUT_String();

                        if (_creg.DirectionFlag)
                            _reg.SI -= 2;
                        else
                            _reg.SI += 2;

                        if (_repeat)
                            _reg.CX--;
                    } while (_reg.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x70] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x71] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x72] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x73] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x74] = new OpCodeRecord(Execute_CondJump);  
            _opTable[0x75] = new OpCodeRecord(Execute_CondJump);  
            _opTable[0x76] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x77] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x78] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x79] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x7a] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x7b] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x7c] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x7d] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x7e] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x7f] = new OpCodeRecord(Execute_CondJump);
            _opTable[0x80] = new OpCodeRecord(Execute_Group1);
            _opTable[0x81] = new OpCodeRecord(Execute_Group1);
            _opTable[0x82] = new OpCodeRecord(Execute_Group1);
            _opTable[0x83] = new OpCodeRecord(Execute_Group1);
            _opTable[0x84] = new OpCodeRecord(ExecuteLogical_General);  // TEST
            _opTable[0x85] = new OpCodeRecord(ExecuteLogical_General);  // TEST
            _opTable[0x86] = new OpCodeRecord(ExecuteXCHG_General);
            _opTable[0x87] = new OpCodeRecord(ExecuteXCHG_General);
            _opTable[0x88] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x89] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x8a] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x8b] = new OpCodeRecord(ExecuteMOV_General);
            _opTable[0x8c] = new OpCodeRecord(ExecuteMOV_SReg);
            _opTable[0x8d] = new OpCodeRecord(Execute_LEA);
            _opTable[0x8e] = new OpCodeRecord(ExecuteMOV_SReg);  // MOV CS - this should never be used in practice
            _opTable[0x8f] = new OpCodeRecord(Execute_POP);
            _opTable[0x90] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x91] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x92] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x93] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x94] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x95] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x96] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x97] = new OpCodeRecord(ExecuteXCHG_AX);
            _opTable[0x98] = new OpCodeRecord(Execute_CBW);
            _opTable[0x99] = new OpCodeRecord(Execute_CWD);
            _opTable[0x9a] = new OpCodeRecord(Execute_CallFar);
            _opTable[0x9b] = new OpCodeRecord(() => { }); // WAIT (for now NOP)
            _opTable[0x9c] = new OpCodeRecord(Execute_PUSH);
            _opTable[0x9d] = new OpCodeRecord(Execute_POP);
            // SAHF - Store SH to flags
            _opTable[0x9e] = new OpCodeRecord(() => { _creg.Register = Util.GetValue16((byte)(_creg.Register >> 8), _reg.AH); });
            // LAHF - Load AH from flags
            _opTable[0x9f] = new OpCodeRecord( () => { _reg.AH = (byte)(_creg.Register & 0x00ff); });
            _opTable[0xa0] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa1] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa2] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa3] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa4] = new OpCodeRecord(Execute_MoveString);
            _opTable[0xa5] = new OpCodeRecord(Execute_MoveString);
            _opTable[0xa6] = new OpCodeRecord(Execute_CompareString);
            _opTable[0xa7] = new OpCodeRecord(Execute_CompareString);
            _opTable[0xa8] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0xa9] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0xaa] = new OpCodeRecord(Execute_StoreString);
            _opTable[0xab] = new OpCodeRecord(Execute_StoreString);
            _opTable[0xac] = new OpCodeRecord(Execute_LoadString);
            _opTable[0xad] = new OpCodeRecord(Execute_LoadString);
            _opTable[0xae] = new OpCodeRecord(Execute_ScanString);
            _opTable[0xaf] = new OpCodeRecord(Execute_ScanString);
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
            // GRP2 rm-8 imm-8 (80186)
            _opTable[0xc0] = new OpCodeRecord(Execute_RotateAndShift);
            // GRP2 rm-16 imm-16 (80186)
            _opTable[0xc1] = new OpCodeRecord(Execute_RotateAndShift);

            _opTable[0xc2] = new OpCodeRecord(() => // ret imm-16
            {
                UInt16 oper = GetImmediate16();
                _bus.IP = Pop();
                _reg.SP += oper;
            });
            _opTable[0xc3] = new OpCodeRecord(() => // ret
            {
                _bus.IP = Pop();
            });
            _opTable[0xc4] = new OpCodeRecord(Execute_LDS_LES);
            _opTable[0xc5] = new OpCodeRecord(Execute_LDS_LES);
            _opTable[0xc6] = new OpCodeRecord(ExecuteMOV_c6);
            _opTable[0xc7] = new OpCodeRecord(ExecuteMOV_c7);
            _opTable[0xc8] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0xc8 is not implemented"); });
            _opTable[0xc9] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0xc9 is not implemented"); });
            _opTable[0xca] = new OpCodeRecord(() => // retf imm-16
            {
                UInt16 oper = GetImmediate16();
                _bus.IP = Pop();
                _bus.CS = Pop();
                _reg.SP += oper;
            });
            _opTable[0xcb] = new OpCodeRecord(() => // retf
            {
                _bus.IP = Pop();
                _bus.CS = Pop();
            });
            _opTable[0xcc] = new OpCodeRecord(() => { Interrupt(3); });
            _opTable[0xcd] = new OpCodeRecord(() => { Interrupt(_bus.NextIP()); });
            _opTable[0xce] = new OpCodeRecord(() => { if (_creg.OverflowFlag) Interrupt(4); });
            _opTable[0xcf] = new OpCodeRecord(() => // iret
            {
                _bus.IP = Pop();
                _bus.CS = Pop();
                _creg.Register = Pop();
            });
            _opTable[0xd0] = new OpCodeRecord(Execute_RotateAndShift);
            _opTable[0xd1] = new OpCodeRecord(Execute_RotateAndShift);
            _opTable[0xd2] = new OpCodeRecord(Execute_RotateAndShift);
            _opTable[0xd3] = new OpCodeRecord(Execute_RotateAndShift);
            _opTable[0xd4] = new OpCodeRecord(Execute_AsciiAdjustMUL);
            _opTable[0xd5] = new OpCodeRecord(Execute_AsciiAdjustDIV);
            // undocumented SALC instruction
            _opTable[0xd6] = new OpCodeRecord(() => { if (_creg.CarryFlag) _reg.AL = 0xff; else _reg.AL = 0x00; });
            _opTable[0xd7] = new OpCodeRecord(Execute_XLAT);

            // D8-DF ESC OPCODE,SOURCE (to math co-processor)
            // these are unsupported but they use the address byte so this should read
            _opTable[0xd8] = new OpCodeRecord(() => { _bus.NextIP(); } );
            _opTable[0xd9] = new OpCodeRecord(() => { _bus.NextIP(); });
            _opTable[0xda] = new OpCodeRecord(() => { _bus.NextIP(); });
            _opTable[0xdb] = new OpCodeRecord(() => { _bus.NextIP(); });
            _opTable[0xdc] = new OpCodeRecord(() => { _bus.NextIP(); });
            _opTable[0xdd] = new OpCodeRecord(() => { _bus.NextIP(); });
            _opTable[0xde] = new OpCodeRecord(() => { _bus.NextIP(); });
            _opTable[0xdf] = new OpCodeRecord(() => { _bus.NextIP(); });

            _opTable[0xe0] = new OpCodeRecord(Execute_Loop);
            _opTable[0xe1] = new OpCodeRecord(Execute_Loop);
            _opTable[0xe2] = new OpCodeRecord(Execute_Loop);
            _opTable[0xe3] = new OpCodeRecord(Execute_JumpCXZ);
            _opTable[0xe4] = new OpCodeRecord(Execute_IN);
            _opTable[0xe5] = new OpCodeRecord(Execute_IN);
            _opTable[0xe6] = new OpCodeRecord(Execute_OUT);
            _opTable[0xe7] = new OpCodeRecord(Execute_OUT);
            _opTable[0xe8] = new OpCodeRecord(Execute_CallNear);
            _opTable[0xe9] = new OpCodeRecord(Execute_JumpNear);
            _opTable[0xea] = new OpCodeRecord(Execute_JumpFar);
            _opTable[0xeb] = new OpCodeRecord(Execute_JumpShort);
            _opTable[0xec] = new OpCodeRecord(Execute_IN);
            _opTable[0xed] = new OpCodeRecord(Execute_IN);
            _opTable[0xee] = new OpCodeRecord(Execute_OUT);
            _opTable[0xef] = new OpCodeRecord(Execute_OUT);
            _opTable[0xf0] = new OpCodeRecord(() => { }); // LOCK
            _opTable[0xf1] = new OpCodeRecord(() => { }); // officially undocumented opcode
            // F2 REPNE/REPNZ
            _opTable[0xf2] = new OpCodeRecord(() =>
            {
                _repeat = true;
                _repeatType = 1;
            });
            // F3 REP/E/Z
            _opTable[0xf3] = new OpCodeRecord(() =>
            {
                _repeat = true;
                _repeatType = 2;
            });
            _opTable[0xf4] = new OpCodeRecord(() => // F4 HLT
            {
                _bus.IP--;
                Halted = true;
            }); 
            _opTable[0xf5] = new OpCodeRecord(() => { _creg.CarryFlag = !_creg.CarryFlag; });  // CMC - complement carry flag
            _opTable[0xf6] = new OpCodeRecord(Execute_Group3);
            _opTable[0xf7] = new OpCodeRecord(Execute_Group3);
            _opTable[0xf8] = new OpCodeRecord(() => { _creg.CarryFlag = false; });  // F8 CLC - clear carry flag
            _opTable[0xf9] = new OpCodeRecord(() => { _creg.CarryFlag = true; });  // F9 STC - set carry flag
            _opTable[0xfa] = new OpCodeRecord(() => { _creg.InterruptEnable = false; });  // FA CLI - clear interrupt flag
            _opTable[0xfb] = new OpCodeRecord(() => { _creg.InterruptEnable = true; });  // FB STI - set interrupt flag
            _opTable[0xfc] = new OpCodeRecord(() => { _creg.DirectionFlag = false; });  // FC CLD - clear direction flag
            _opTable[0xfd] = new OpCodeRecord(() => { _creg.DirectionFlag = true; });  // FD STD - set direction flag
            _opTable[0xfe] = new OpCodeRecord(Execute_Group4);
            _opTable[0xff] = new OpCodeRecord(Execute_Group5);
        }

        private void Execute_XLAT()
        {
            // one byte instruction (0xd7)
            // Store into AL the value located at a 256 byte lookup table
            // BX is the beginning of the table and AL is the offset
            _reg.AL = _bus.GetData8(_reg.BX + _reg.AL);
        }

        private void Execute_CBW()
        {
            if( (_reg.AL & 0x80) == 0x80 )
            {
                _reg.AH = 0xff;
            }
            else
            {
                _reg.AH = 0x00;
            }
        }
        private void Execute_CWD()
        {
            if( (_reg.AX & 0x8000) == 0x8000 )
            {
                _reg.DX = 0xffff;
            }
            else
            {
                _reg.DX = 0;
            }
        }

        private void Execute_Loop()
        {
            UInt16 tmp = SignExtend(_bus.NextIP());
            _reg.CX--;
            if( _reg.CX != 0)
            {
                if ((_currentOP == 0xe0 && !_creg.ZeroFlag) ||  // LOOPNZ
                    (_currentOP == 0xe1 && _creg.ZeroFlag) ||   // LOOPZ
                    (_currentOP == 0xe2))                       // LOOP
                {
                    _bus.IP += tmp;
                }
            }
        }

        #region Instructions 60-6F (80186)

        private void Execute_PUSHA()
        {
            // Op 0x60
            UInt16 old_sp = _reg.SP;
            Push(_reg.AX);
            Push(_reg.CX);
            Push(_reg.DX);
            Push(_reg.BX);
            Push(old_sp);
            Push(_reg.BP);
            Push(_reg.SI);
            Push(_reg.DI);
        }

        private void Execute_POPA()
        {
            // Op 0x61
            _reg.DI = Pop();
            _reg.SI = Pop();
            _reg.BP = Pop();
            UInt16 new_sp = Pop();
            _reg.BX = Pop();
            _reg.DX = Pop();
            _reg.CX = Pop();
            _reg.AX = Pop();
        }

        private void Execute_Bound()
        {
            // Op 0x62
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            // for now assume OK

            //int offset = GetSourceData(0, 1, mod, reg, rm);
            //UInt16 source = _bus.GetData16(offset);

            //offset = GetDestinationData(0, 1, mod, reg, rm);
            //UInt16 dest = _bus.GetData16(offset);

            //if( SignExtend32(source) < SignExtend32(dest) )
            //{

            //}
        }


        #endregion

        #region Input/Output device instructions

        // IN   data from port is stored in the accumulator
        private void Execute_IN()
        {
            IInputDevice device;

            UInt16 port;
            if (_currentOP == 0xec || _currentOP == 0xed)
                port = _reg.DX;
            else
                port = _bus.NextIP();

            int word_size = GetWordSize();

            if (_inputDevices.TryGetValue(port, out device))
            {
                if (word_size == 0)
                    _reg.AL = device.Read();
                else
                    _reg.AX = device.Read16();
            }
            else
            {
                // if no device is attached, zero out the register
                if (word_size == 0)
                    _reg.AL = 0;
                else
                    _reg.AX = 0;
            }
        }

        private void Execute_IN_String()
        {
            IInputDevice device;
            int word_size = GetWordSize();

            if (_inputDevices.TryGetValue(_reg.DX, out device))
            {
                if (word_size == 0)
                    _bus.StoreString8(_reg.DI, device.Read());
                else
                    _bus.StoreString16(_reg.DI, device.Read16());
            }
            else
            {
                if (word_size == 0)
                    _bus.StoreString8(_reg.DI, 0);
                else
                    _bus.StoreString16(_reg.DI, 0);
            }
        }   

        private void Execute_OUT_String()
        {
            IOutputDevice device;
            int word_size = GetWordSize();

            if(_outputDevices.TryGetValue(_reg.DX, out device))
            {
                if (word_size == 0)
                    device.Write(_bus.GetDestString8(_reg.SI));
                else
                    device.Write16(_bus.GetDestString16(_reg.SI));
            }
        }

        // OUT  accumulator is written out to the port
        private void Execute_OUT()
        {
            IOutputDevice device;

            UInt16 port;
            if (_currentOP == 0xee || _currentOP == 0xef)
                port = _reg.DX;
            else
                port = _bus.NextIP();

            int word_size = GetWordSize();

            if (_outputDevices.TryGetValue(port, out device))
            {
                if (word_size == 0)
                    device.Write(_reg.AL);
                else
                    device.Write16(_reg.AX);
            }
        }
        #endregion

        #region Grouped Instructions
        private void Execute_Group1()
        {
            // These op codes are grouped instructions with reg field defining the operator.
            // 0x80 are 8 bit operations 
            // 0x81 are 16 bit operations
            // 0x82 is functionally the same as 0x80
            // 0x83 uses sign extention for the 8 bit immediate data

            int word_size = 0;
            if (_currentOP == 0x81 || _currentOP == 0x83)
            {
                word_size = 1;
            }

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            AssertMOD(mod);
            AssertREG(reg);
            AssertRM(rm);


            // If displacement offset is needed, those bytes will appear before the immediate data so we need to retrieve that first.
            // The offset isn't needed within here but we need to retrieve it first.
            int dest = 0;
            if (mod == 0x00)
            {
                dest = GetRMTable1(rm);
            }
            else if ((mod == 0x01) || (mod == 0x02))
            {
                dest = GetRMTable2(mod, rm);
            }

            // Get immediate data
            int source = 0;

            if (_currentOP == 0x83)
            {
                source = SignExtend(_bus.NextIP());
            }
            else if (word_size == 0)
            {
                source = _bus.NextIP();

            }
            else
            {
                source = GetImmediate16();
            }

            // check for invalid instructions
            // the 8086 manual states the following are invalid for op codes 0x82 and 0x83:
            //      reg=001
            //      reg=100
            //      reg=110
            //if ((_currentOP == 0x82 || _currentOP == 0x83) &&
            //    (reg == 0x01 || reg == 0x04 || reg == 0x06))
            //{
            //    throw new ArgumentOutOfRangeException("reg", reg, string.Format("Invalid reg value in opcode={0:X2}", _currentOP));
            //}

            // reg field defines the operation
            switch (reg)
            {
                case 0x00: // ADD R/M, IMM
                    {
                        ADD_Destination(source, 0, word_size, mod, reg, rm, false);
                        break;
                    }
                case 0x01: // OR R/M, IMM
                    {
                        OR_Destination(source, 0, word_size, mod, reg, rm);
                        break;
                    }
                case 0x02: // ADC R/M, IMM
                    {
                        ADD_Destination(source, 0, word_size, mod, reg, rm, true);
                        break;
                    }
                case 0x03: // SBB R/M, IMM
                    {
                        SUB_Destination(source, 0, word_size, mod, reg, rm, true, false);
                        break;
                    }
                case 0x04: // AND R/M, IMM
                    {
                        AND_Destination(source, 0, word_size, mod, reg, rm, false);
                        break;
                    }
                case 0x05: // SUB R/M, IMM
                    {
                        SUB_Destination(source, 0, word_size, mod, reg, rm, false, false);
                        break;
                    }
                case 0x06: // XOR R/M, IMM
                    {
                        XOR_Destination(source, 0, word_size, mod, reg, rm);
                        break;
                    }
                case 0x07: // CMP R/M, IMM
                    {
                        SUB_Destination(source, 0, word_size, mod, reg, rm, false, true);
                        break;
                    }
            }

        }

        // This is group 2
        private void Execute_RotateAndShift()
        {
            /*
                D0:  <op> reg/mem-8, 1
                D1:  <op> reg/mem-16, 1
                D2:  <op> reg/mem-8, CL
                D3:  <op> reg/mem-16, CL

                REG 000: ROL - rotate left
                REG 001: ROR - rotate right 
                REG 002: RCL - rotate through carry left  (CF is low order bit)
                REG 003: RCR - rotate through carry right (CF is low order bit)
                REG 004: SAL/SHL - shift left (zero padded) (OF is cleared if sign bit is unchanged)
                REG 005: SHR - shift right (zero padded) (OF is cleared if sign bit is unchanged)
                REG 006: NOT USED
                REG 007: SAR - shift arithmetic right

                Shifts affect PF, SF, ZF.  CF is the last bit shifted out.
                    OF is defined only for a single bit shift.

                Rotate affects CF and OF only.

            */

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int word_size = GetWordSize();

            int operand2 = 1;
            if (_currentOP == 0xd2 || _currentOP == 0xd3)
                operand2 = _reg.CL;
            else if (_currentOP == 0xc0 || _currentOP == 0xc1)
            {
                // in this case we need to read the RM data first in the case there
                // is displacement data to read before the immediate data
                int operand1 = GetSourceData(0, word_size, mod, reg, rm);
                operand2 = _bus.NextIP();
            }

            switch (reg)
            {
                case 0x00:
                    {
                        RotateLeft(word_size, operand2, mod, rm, false, false);
                        break;
                    }
                case 0x01:
                    {
                        RotateRight(word_size, operand2, mod, rm, false, false, false);
                        break;
                    }
                case 0x02:
                    {
                        RotateLeft(word_size, operand2, mod, rm, true, false);
                        break;
                    }
                case 0x03:
                    {
                        RotateRight(word_size, operand2, mod, rm, true, false, false);
                        break;
                    }
                case 0x04:
                    {
                        RotateLeft(word_size, operand2, mod, rm, false, true);
                        break;
                    }
                case 0x05:
                    {
                        RotateRight(word_size, operand2, mod, rm, false, true, false);
                        break;
                    }
                case 0x07:
                    {
                        RotateRight(word_size, operand2, mod, rm, false, true, true);
                        break;
                    }
            }
        }

        private void Execute_Group3()
        {
            /*
                REG 000: TEST R/M-8, IMM-8
                REG 001: NOT USED
                REG 002: NOT R/M-8
                REG 003: NEG R/M-8
                REG 004: MUL R/M-8
                REG 005: IMUL R/M-8
                REG 006: DIV R/M-8
                REG 007: IDIV R/M-8
            */

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int word_size = GetWordSize();
            int direction = GetDirection();

            int source = GetSourceData(direction, word_size, mod, reg, rm);
            int dest = 0;
            int result = 0;

            switch (reg)
            {
                case 0x00: // TEST
                    {
                        if (word_size == 0)
                            dest = _bus.NextIP();
                        else
                            dest = GetImmediate16();

                        result = source & dest;
                        // Flags: O S Z A P C
                        //        0 x x ? x 0
                        _creg.OverflowFlag = false;
                        _creg.CarryFlag = false;
                        _creg.CalcSignFlag(word_size, result);
                        _creg.CalcZeroFlag(word_size, result);
                        _creg.CalcParityFlag(result);
                        break;
                    }
                case 0x01: // NOT USED
                    {
                        break;
                    }
                case 0x02: // NOT (no flags)
                    {
                        SaveToDestination(~source, 0, word_size, mod, reg, rm);
                        break;
                    }
                case 0x03: // NEG (CF, ZF, SF, OF, PF, AF)
                    {
                        result = (~source) + 1;
                        SaveToDestination(result, 0, word_size, mod, reg, rm);

                        _creg.CalcOverflowFlag(word_size, 0, result);
                        _creg.CalcSignFlag(word_size, result);
                        _creg.CalcZeroFlag(word_size, result);
                        _creg.CalcAuxCarryFlag(source, dest);
                        _creg.CalcParityFlag(result);
                        _creg.CalcCarryFlag(word_size, result);
                        break;
                    }
                case 0x04: // MUL
                    {
                        if (word_size == 0)
                        {
                            result = source * _reg.AL;
                            _reg.AX = (UInt16)result;

                            _creg.CarryFlag = (_reg.AH != 0);
                            _creg.OverflowFlag = _creg.CarryFlag;
                        }
                        else
                        {
                            result = source * _reg.AX;
                            _reg.DX = (UInt16)(result >> 16);
                            _reg.AX = (UInt16)(result);

                            _creg.CarryFlag = (_reg.DX != 0);
                            _creg.OverflowFlag = _creg.CarryFlag;
                        }
                        break;
                    }
                case 0x05: // IMUL
                    {
                        if (word_size == 0)
                        {
                            _reg.AX = (UInt16)(SignExtend((byte)source) * SignExtend(_reg.AL));

                            if ((_reg.AL & 0x80) == 0x80)
                                _creg.CarryFlag = (_reg.AH != 0xff);
                            else
                                _creg.CarryFlag = (_reg.AH != 0x00);

                            _creg.OverflowFlag = _creg.CarryFlag;
                        }
                        else
                        {
                            result = (int)(SignExtend((byte)source) * SignExtend32(_reg.AX));
                            _reg.DX = (UInt16)(result >> 16);
                            _reg.AX = (UInt16)(result);

                            if ((_reg.AX & 0x8000) == 0x8000)
                                _creg.CarryFlag = (_reg.DX != 0xffff);
                            else
                                _creg.CarryFlag = (_reg.DX != 0x0000);

                            _creg.OverflowFlag = _creg.CarryFlag;
                        }
                        break;
                    }
                case 0x06: // DIV
                    {
                        if (word_size == 0)
                        {
                            result = (byte)(_reg.AX / source);
                            _reg.AH = (byte)(_reg.AX % source);
                            _reg.AL = (byte)(result);
                        }
                        else
                        {
                            dest = (_reg.DX << 16) | _reg.AX;
                            _reg.AX = (UInt16)(dest / source);
                            _reg.DX = (UInt16)(dest % source);
                        }
                        break;
                    }
                case 0x07: // IDIV
                    {
                        if (word_size == 0)
                        {

                            UInt16 s1 = _reg.AX;
                            UInt16 s2 = (UInt16)source;

                            bool sign = ((s1 ^ s2) & 0x8000) == 0x8000;

                            if (s1 >= 0x8000)
                                s1 = (UInt16)(~s1 + 1);
                            if (s2 >= 0x8000)
                                s2 = (UInt16)(~s2 + 1);

                            UInt16 d1 = (UInt16)(s1 / s2);
                            UInt16 d2 = (UInt16)(s1 % s2);

                            if (sign)
                            {
                                d1 = (UInt16)(~d1 + 1);
                                d2 = (UInt16)(~d2 + 1);
                            }

                            _reg.AL = (byte)d1;
                            _reg.AH = (byte)d2;
                        }
                        else
                        {

                            UInt32 dxax = (UInt32)((_reg.DX << 16) | _reg.AX);
                            UInt32 divisor = SignExtend32((UInt16)source);

                            bool sign = ((dxax ^ divisor) & 0x80000000) == 0x80000000;

                            if (dxax >= 0x80000000)
                                dxax = (UInt32)(~dxax + 1);

                            if (divisor >= 0x80000000)
                                divisor = (UInt32)(~divisor + 1);

                            UInt32 d1 = (UInt32)(dxax / divisor);
                            UInt32 d2 = (UInt32)(dxax % divisor);

                            if (sign)
                            {
                                d1 = (UInt32)((~d1 + 1) & 0xffff);
                                d2 = (UInt32)((~d2 + 1) & 0xffff);
                            }

                            _reg.AX = (UInt16)d1;
                            _reg.DX = (UInt16)d2;
                        }
                        break;
                    }
            }
        }

        private void Execute_Group4()
        {
            // REG 000: INC reg/mem-8
            // REG 001: DEC reg/mem-8
            // all others are undefined

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int word_size = GetWordSize();

            if( reg == 0x00 )
            {
                Execute_Increment(word_size, mod, reg, rm);
            }
            else if( reg == 0x01 )
            {
                Execute_Decrement(word_size, mod, reg, rm);
            }
            else
                throw new ArgumentOutOfRangeException("reg", reg, string.Format("Invalid reg value {0} in opcode={1:X2}", reg, _currentOP));
        }

        private void Execute_Group5()
        {
            // REG 000: INC mem-16
            // REG 001: DEC mem-16
            // REG 002: CALL reg/mem-16
            // REG 003: CALL mem-16
            // REG 004: JMP reg/mem-16
            // REG 005: JMP mem-16
            // REG 006: PUSH mem-16
            // REG 007: undefined

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int word_size = GetWordSize();
            int oper = GetDestinationData(0, word_size, mod, reg, rm);

            switch(reg)
            {
                case 0x00:
                    {
                        Execute_Increment(word_size, mod, reg, rm);
                        break;
                    }
                case 0x01:
                    {
                        Execute_Decrement(word_size, mod, reg, rm);
                        break;
                    }
                case 0x02:
                    {
                        // CALL reg/mem-16 (intrasegment)
                        Push(_bus.IP);
                        _bus.IP = (UInt16) oper;
                        break;
                    }
                case 0x03:
                    {
                        // CALL mem-16 (intersegment)
                        Push(_bus.CS);
                        Push(_bus.IP);
                        _bus.IP = _bus.GetData16(oper);
                        _bus.CS = _bus.GetData16(oper + 2);
                        break;
                    }
                case 0x04:
                    {
                        // JMP reg/mem=16 (intrasegment)
                        _bus.IP = (UInt16) oper;
                        break;
                    }
                case 0x05:
                    {
                        // JMP mem-16 (intersegment)
                        _bus.IP = _bus.GetData16(oper);
                        _bus.CS = _bus.GetData16(oper + 2);
                        break;
                    }
                case 0x06:
                    {
                        // push mem-16
                        Push((UInt16)oper);
                        break;
                    }
            }
        }

        #endregion

        #region Jump/Call Instructions

        private void Execute_CallNear()
        {
            UInt16 oper = GetImmediate16();
            Push(_bus.IP);
            _bus.IP += oper;
        }

        private void Execute_CallFar()
        {
            UInt16 nextIP = GetImmediate16();
            UInt16 nextCS = GetImmediate16();
            Push(_bus.CS);
            Push(_bus.IP);
            _bus.IP = nextIP;
            _bus.CS = nextCS;
        }

        private void Execute_CondJump()
        {
            // JMP does not save anything to the stack and no return is expected.
            // Intrasegment JMP changes IP by adding the relative displacement from the instruction.

            // JMP IP-INC8  8 bit signed increment to the instruction pointer
            bool jumping = false;
            switch (_currentOP)
            {
                case 0x70:
                    {
                        jumping = _creg.OverflowFlag;
                        break;
                    }
                case 0x71:
                    {
                        jumping = !_creg.OverflowFlag;
                        break;
                    }
                case 0x72:
                    {
                        jumping = _creg.CarryFlag;
                        break;
                    }
                case 0x73:
                    {
                        jumping = !_creg.CarryFlag;
                        break;
                    }
                case 0x74:
                    {
                        jumping = _creg.ZeroFlag;
                        break;
                    }
                case 0x75:
                    {
                        jumping = !_creg.ZeroFlag;
                        break;
                    }
                case 0x76:
                    {
                        jumping = (_creg.CarryFlag | _creg.ZeroFlag);
                        break;
                    }
                case 0x77:
                    {
                        jumping = !(_creg.CarryFlag | _creg.ZeroFlag);
                        break;
                    }
                case 0x78:
                    {
                        jumping = _creg.SignFlag;
                        break;
                    }
                case 0x79:
                    {
                        jumping = !_creg.SignFlag;
                        break;
                    }
                case 0x7a:
                    {
                        jumping = _creg.ParityFlag;
                        break;
                    }
                case 0x7b:
                    {
                        jumping = !_creg.ParityFlag;
                        break;
                    }
                case 0x7c:
                    {
                        jumping = (_creg.SignFlag ^ _creg.OverflowFlag);
                        break;
                    }
                case 0x7d:
                    {
                        jumping = !(_creg.SignFlag ^ _creg.OverflowFlag);
                        break;
                    }
                case 0x7e:
                    {
                        jumping = ((_creg.SignFlag ^ _creg.OverflowFlag) | _creg.ZeroFlag);
                        break;
                    }
                case 0x7f:
                    {
                        jumping = !((_creg.SignFlag ^ _creg.OverflowFlag) | _creg.ZeroFlag);
                        break;
                    }
            }
            if (jumping)
                Execute_JumpShort();
            else
                _bus.NextIP();  
        }
        private void Execute_JumpCXZ()
        {
            if (_reg.CX == 0)
                Execute_JumpShort();
            else
                _bus.NextIP();
        }
        private void Execute_JumpShort()
        {
            // add to IP "after" the instruction is complete
            UInt16 oper = SignExtend(_bus.NextIP());
            _bus.IP += oper;
        }
        private void Execute_JumpNear()
        {
            UInt16 oper = GetImmediate16();
            _bus.IP += oper;
        }
        private void Execute_JumpFar()
        {
            UInt16 nextIP = GetImmediate16();
            UInt16 nextCS = GetImmediate16();
            _bus.IP = nextIP;
            _bus.CS = nextCS;
        }

        #endregion  

        #region String Instructions
        private void Execute_MoveString()
        {
            do
            {
                int word_size = GetWordSize();
                if (word_size == 0)
                {
                    _bus.MoveString8(_reg.SI, _reg.DI);
                    if (_creg.DirectionFlag)
                    {
                        _reg.SI--;
                        _reg.DI--;
                    }
                    else
                    {
                        _reg.SI++;
                        _reg.DI++;
                    }
                }
                else
                {
                    _bus.MoveString16(_reg.SI, _reg.DI);
                    if (_creg.DirectionFlag)
                    {
                        _reg.SI -= 2;
                        _reg.DI -= 2;
                    }
                    else
                    {
                        _reg.SI += 2;
                        _reg.DI += 2;
                    }
                }
                if (_repeat) _reg.CX--;

                if (_repeatType == 1 && !_creg.ZeroFlag)
                    _repeat = false;
                if (_repeatType == 2 && _creg.ZeroFlag)
                    _repeat = false;

            } while (_repeat && _reg.CX != 0);
            _repeat = false;
        }
        private void Execute_CompareString()
        {
            int word_size = GetWordSize();
            int result = 0;
            int source = 0;
            int dest = _bus.GetData(word_size, _reg.SI);

            // NOTE: the concept of "source" and "dest" for subtraction here is flipped.

            do
            {
                if (word_size == 0)
                {
                    source = _bus.GetDestString8(_reg.DI);
                    if (_creg.DirectionFlag)
                    {
                        _reg.SI--;
                        _reg.DI--;
                    }
                    else
                    {
                        _reg.SI++;
                        _reg.DI++;
                    }
                }
                else
                {
                    source = _bus.GetDestString16(_reg.DI);
                    if (_creg.DirectionFlag)
                    {
                        _reg.SI -= 2;
                        _reg.DI -= 2;
                    }
                    else
                    {
                        _reg.SI += 2;
                        _reg.DI += 2;
                    }
                }

                result = dest - source;
                _creg.CalcOverflowFlag(word_size, source, dest);
                _creg.CalcSignFlag(word_size, result);
                _creg.CalcZeroFlag(word_size, result);
                _creg.CalcAuxCarryFlag(source, dest);
                _creg.CalcParityFlag(result);
                _creg.CalcCarryFlag(word_size, result);
                if (_repeat) _reg.CX--;

                if (_repeatType == 1 && !_creg.ZeroFlag)
                    _repeat = false;
                if (_repeatType == 2 && _creg.ZeroFlag)
                    _repeat = false;

            } while (_repeat && _reg.CX != 0);
            _repeat = false;
        }
        private void Execute_ScanString()
        {
            int word_size = GetWordSize();
            int result = 0;
            int source = 0;
            int dest = 0;

            do
            {
                if (word_size == 0)
                {
                    dest = _bus.GetDestString8(_reg.DI);
                    source = _reg.AL;
                    if (_creg.DirectionFlag)
                    {
                        _reg.DI--;
                    }
                    else
                    {
                        _reg.DI++;
                    }
                }
                else
                {
                    dest = _bus.GetDestString16(_reg.DI);
                    source = _reg.AX;
                    if (_creg.DirectionFlag)
                    {
                        _reg.DI -= 2;
                    }
                    else
                    {
                        _reg.DI += 2;
                    }
                }

                result = dest - source;
                _creg.CalcOverflowFlag(word_size, source, dest);
                _creg.CalcSignFlag(word_size, result);
                _creg.CalcZeroFlag(word_size, result);
                _creg.CalcAuxCarryFlag(source, dest);
                _creg.CalcParityFlag(result);
                _creg.CalcCarryFlag(word_size, result);

                if (_repeat) _reg.CX--;

                if (_repeatType == 1 && !_creg.ZeroFlag)
                    _repeat = false;
                if (_repeatType == 2 && _creg.ZeroFlag)
                    _repeat = false;

            } while (_repeat && _reg.CX != 0);
            _repeat = false;
        }
        private void Execute_StoreString()
        {
            int word_size = GetWordSize();
            do
            {
                if (word_size == 0)
                {
                    _bus.StoreString8(_reg.DI, _reg.AL);
                    if (_creg.DirectionFlag)
                    {
                        _reg.DI--;
                    }
                    else
                    {
                        _reg.DI++;
                    }
                }
                else
                {
                    _bus.StoreString16(_reg.DI, _reg.AX);
                    if (_creg.DirectionFlag)
                    {
                        _reg.DI -= 2;
                    }
                    else
                    {
                        _reg.DI += 2;
                    }
                }
                if (_repeat) _reg.CX--;
            } while (_repeat && _reg.CX != 0);
            _repeat = false;
        }
        private void Execute_LoadString()
        {
            int word_size = GetWordSize();
            do
            {
                if (word_size == 0)
                {
                    _reg.AL = _bus.GetData8(_reg.SI);
                    if (_creg.DirectionFlag)
                    {
                        _reg.SI--;
                    }
                    else
                    {
                        _reg.SI++;
                    }
                }
                else
                {
                    _reg.AX = _bus.GetData16(_reg.SI);
                    if (_creg.DirectionFlag)
                    {
                        _reg.SI -= 2;
                    }
                    else
                    {
                        _reg.SI += 2;
                    }
                }
                if (_repeat) _reg.CX--;
            } while (_repeat && _reg.CX != 0);
            _repeat = false;
        }
        #endregion

        #region BCD adjustment instructions
        private void Execute_DecimalAdjustADD()
        {
            byte old_al = _reg.AL;
            bool old_cf = _creg.CarryFlag;

            if( ( _reg.AL & 0x0f ) > 9 || _creg.AuxCarryFlag )
            {
                _creg.CalcCarryFlag(0, _reg.AL+6);
                _creg.CarryFlag = old_cf | _creg.CarryFlag;
                _creg.AuxCarryFlag = true;

                _reg.AL += 6;
            }
            else
            {
                _creg.AuxCarryFlag = false;
            }

            if ((old_al > 0x99) | old_cf)
            {
                _reg.AL += 0x60;
                _creg.CarryFlag = true;
            }
            else
            {
                _creg.CarryFlag = false;
            }

            _creg.CalcSignFlag(0, _reg.AL);
            _creg.CalcZeroFlag(0, _reg.AL);
            _creg.CalcParityFlag(_reg.AL);
        }
        private void Execute_DecimalAdjustSUB()
        {
            byte old_al = _reg.AL;
            bool old_cf = _creg.CarryFlag;

            if ((_reg.AL & 0x0f) > 9 || _creg.AuxCarryFlag)
            {
                _creg.CalcCarryFlag(0, _reg.AL - 6);
                _creg.CarryFlag = old_cf | _creg.CarryFlag;
                _creg.AuxCarryFlag = true;

                _reg.AL -= 6;
            }
            else
            {
                _creg.AuxCarryFlag = false;
            }

            if ((old_al > 0x99) | old_cf)
            {
                _reg.AL -= 0x60;
                _creg.CarryFlag = true;
            }
            else
            {
                _creg.CarryFlag = false;
            }

            _creg.CalcSignFlag(0, _reg.AL);
            _creg.CalcZeroFlag(0, _reg.AL);
            _creg.CalcParityFlag(_reg.AL);
        }
        private void Execute_AsciiAdjustADD()
        {
            if ((_reg.AL > 9) | _creg.AuxCarryFlag)
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

            // clear high nibble of AL
            _reg.AL = (byte)(_reg.AL & 0x0f);
        }
        private void Execute_AsciiAdjustSUB()
        {
            if ((_reg.AL > 9) | _creg.AuxCarryFlag)
            {
                _reg.AL -= 6;
                _reg.AH -= 1;
                _creg.AuxCarryFlag = true;
                _creg.CarryFlag = true;
            }
            else
            {
                _creg.AuxCarryFlag = false;
                _creg.CarryFlag = false;
            }

            // clear high nibble of AL
            _reg.AL = (byte)(_reg.AL & 0x0f);
        }
        private void Execute_AsciiAdjustMUL()
        {
            _reg.AH = (byte)(_reg.AL / 10);
            _reg.AL = (byte)(_reg.AL % 10);
            _creg.CalcParityFlag(_reg.AX);
            _creg.CalcSignFlag(1, _reg.AX);
            _creg.CalcZeroFlag(1, _reg.AX);
        }
        private void Execute_AsciiAdjustDIV()
        {
            _reg.AL += (byte)(_reg.AH * 10);
            _reg.AH = 0;
            _creg.CalcParityFlag(_reg.AX);
            _creg.CalcSignFlag(1, _reg.AX);
            _creg.CalcZeroFlag(1, _reg.AX);
        }
        #endregion

        #region Address object instructions

        private void Execute_LDS_LES()
        {
            // Transfer a 32 bit pointer variable from the source operand (which must be memory)
            // to the destination operand and DS.
            // The offset word of the pointer is transferred to the destination operand.
            // The segment word of the pointer is transferred to register DS.

            /*
                0xc4 LES reg-16, mem-16
                0xc5 LDS reg-16, mem-16
                     OP MODREGR/M (DISP-LO) (DISP-HI)
            */

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int offset = 0;

            AssertMOD(mod);
            switch (mod)
            {
                case 0x00:
                    {
                        offset = _bus.GetData16(GetRMTable1(rm));
                        break;
                    }
                case 0x01:
                case 0x02:   // difference is processed in the GetRMTable2 function
                    {
                        offset = _bus.GetData16(GetRMTable2(mod, rm));
                        break;
                    }
                case 0x03:
                    {
                        throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", _currentOP));
                    }
            }

            SaveRegField16(reg, _bus.GetData16(offset));

            if (_currentOP == 0xc4)
                _bus.ES = _bus.GetData16(offset + 2);
            else
                _bus.DS = _bus.GetData16(offset + 2);
        }

        private void Execute_LEA()
        {
            // (0x8d) LEA MODREGR/M REG-16, MEM-16, (DISP-LO), (DISP-HI)
            // no flags
            // loads the offset of the source (rather than its value) and stores it in the destination

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);
            int direction = GetDirection();
            int word_size = GetWordSize();

            int offset = GetSourceData(direction, word_size, mod, reg, rm);
            int source = _bus.GetData(word_size, offset);
            SaveToDestination(source, direction, word_size, mod, reg, rm);
        }

        #endregion

        #region PUSH and POP
        private void Execute_PUSH()
        {
            if (_currentOP == 0x9c) // PUSHF (no address byte)
            {
                Push(_creg.Register);
            }
            else
            {
                byte mod = 0, reg = 0, rm = 0;
                // the RM portion of the op code determines the register to use
                SplitAddrByte(_currentOP, ref mod, ref reg, ref rm);

                // for segment register ops, use the reg field
                if (_currentOP < 0x50)
                {
                    Push(GetSegRegField(reg));
                }
                // else use rm field to determine the register
                else
                {
                    Push(GetRegField16(rm));
                }
            }
        }

        private void Execute_POP()
        {
            if (_currentOP == 0x9d) // POPF (no address byte)
            {
                _creg.Register = Pop();
            }
            else
            {
                byte mod = 0, reg = 0, rm = 0;
                SplitAddrByte(_currentOP, ref mod, ref reg, ref rm);
               
                if(_currentOP == 0x8f)
                    SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);
                else
                    SplitAddrByte(_currentOP, ref mod, ref reg, ref rm);


                // for segment register ops, use the reg field
                if (_currentOP < 0x50)
                {
                    SaveSegRegField(rm, Pop());
                }
                else if (_currentOP == 0x8f) // POP R/M-16
                {
                    SaveToDestination(Pop(), 0, 1, mod, reg, rm);
                }
                else
                {
                    SaveRegField16(rm, Pop());
                }
            }
        }
        #endregion

        #region ADD and ADC instructions

        private void ExecuteADD_General()
        {
            // Op Codes: 00-03, 10-13
            // operand1 = operand1 + operand2
            // Flags: O S Z A P C

            // op codes 10-13 are ADC
            bool with_carry = (_currentOP & 0x10) == 0x10;

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = GetDirection();
            int word_size = GetWordSize();

            int source = GetSourceData(direction, word_size, mod, reg, rm);

            ADD_Destination(source, direction, word_size, mod, reg, rm, with_carry);
        }

        private void ExecuteADD_Immediate()
        {
            // Op Codes: 04-05, 14-15
            // operand1 = operand1 + operand2
            // Flags: O S Z A P C

            // op codes 14-15 are ADC
            bool with_carry = (_currentOP & 0x10) == 0x10;

            int direction = 0;
            int word_size = GetWordSize();
            int source;

            if (word_size == 0)
                source = _bus.NextIP();
            else
                source = GetImmediate16();

            // Set flags so the result gets stored in the proper accumulator (AL or AX)
            ADD_Destination(source, direction, word_size, 0x03, 0x00, 0x00, with_carry);
        }

        #endregion

        #region SUB and SBB instructions

        private void ExecuteSUB_General()
        {
            // Op Codes: SUB 28-2B, SBB 18-1B
            //         : CMP 38-3B
            // operand1 = operand1 - operand2
            // Flags: O S Z A P C

            bool with_carry = (_currentOP & 0x10) == 0x10;
            bool comp_only = (_currentOP & 0x30) == 0x30;

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = GetDirection();
            int word_size = GetWordSize();

            int source = GetSourceData(direction, word_size, mod, reg, rm);
            SUB_Destination(source, direction, word_size, mod, reg, rm, with_carry, comp_only);
        }

        private void ExecuteSUB_Immediate()
        {
            // Op Codes: SUB 2C-2D, SBB 1C-1D, CMP 3C-3D
            // operand1 = operand1 - operand2
            // Flags: O S Z A P C
            bool with_carry = (_currentOP & 0x10) == 0x10;
            bool comp_only = (_currentOP & 0x30) == 0x30;

            int direction = 0;
            int word_size = GetWordSize();
            int source;

            if (word_size == 0)
                source = _bus.NextIP();
            else
                source = GetImmediate16();

            // Set flags so the result gets stored in the proper accumulator (AL or AX)
            SUB_Destination(source, direction, word_size, 0x03, 0x00, 0x00, with_carry, comp_only);
        }

        #endregion

        #region Logical Operators (AND, OR, XOR)
        private void ExecuteLogical_General()
        {
            //  OR: 08-0B
            // AND: 20-23
            // XOR: 30-33
            // TEST: 84-85

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = GetDirection();
            int word_size = GetWordSize();

            int source = GetSourceData(direction, word_size, mod, reg, rm);

            // test op code to determine the operator
            switch( (byte)(_currentOP >> 4))
            {
                case 0x00: // OR
                    {
                        OR_Destination(source, direction, word_size, mod, reg, rm);
                        break;
                    }
                case 0x02: // AND
                    {
                        AND_Destination(source, direction, word_size, mod, reg, rm, false);
                        break;
                    }
                case 0x03: // XOR
                    {
                        XOR_Destination(source, direction, word_size, mod, reg, rm);
                        break;
                    }
                case 0x08: // TEST
                    {
                        AND_Destination(source, direction, word_size, mod, reg, rm, true);
                        break;
                    }
            }
        }

        private void ExecuteLogical_Immediate()
        {
            //  OR: 0C-0D
            // AND: 24-25
            // XOR: 34-35
            // TEST: A8-A9

            int direction = 0;
            int word_size = GetWordSize();

            int source;

            if (word_size == 0)
                source = _bus.NextIP();
            else
                source = GetImmediate16();

            // test op code to determine the operator
            switch ((byte)(_currentOP >> 4))
            {
                case 0x00: // OR
                    {
                        OR_Destination(source, direction, word_size, 0x03, 0x00, 0x00);
                        break;
                    }
                case 0x02: // AND
                    {
                        AND_Destination(source, direction, word_size, 0x03, 0x00, 0x00, false);
                        break;
                    }
                case 0x03: // XOR
                    {
                        XOR_Destination(source, direction, word_size, 0x03, 0x00, 0x00);
                        break;
                    }
                case 0x0a: // TEST
                    {
                        AND_Destination(source, direction, word_size, 0x03, 0x00, 0x00, true);
                        break;
                    }
            }
        }
        #endregion

        #region Increment and Decrement
        private void ExecuteINC()
        {
            // Op Codes: 40-47 INC

            // Parse the op code, the affected register is the last 3 bits
            // (similiar to using RM as REG field)
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_currentOP, ref mod, ref reg, ref rm);

            UInt16 dest = GetRegField16(rm);
            UInt16 result = (UInt16)(dest + 1);
            SaveRegField16(rm, result);

            // Flags: O S Z A P
            // Flags are set as if ADD instruction was used with operand2 = 1
            // Carry flag is not affected by increment
            _creg.CalcOverflowFlag(1, 0x01, dest);
            _creg.CalcSignFlag(1, result);
            _creg.CalcZeroFlag(1, result);
            _creg.CalcAuxCarryFlag(0x01, dest);
            _creg.CalcParityFlag(result);
        }

        private void ExecuteDEC()
        {
            // Op Codes: 48-4F DEC

            // Parse the op code, the affected register is the last 3 bits
            // (similiar to using RM as REG field)
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_currentOP, ref mod, ref reg, ref rm);

            UInt16 dest = GetRegField16(rm);
            UInt16 result = (UInt16)(dest - 1);
            SaveRegField16(rm, result);

            // Flags: O S Z A P
            // Flags are set as if SUB instruction was used with operand2 = 1
            // Carry flag is not affected by decrement
            _creg.CalcOverflowSubtract(1, 0x01, dest);
            _creg.CalcSignFlag(1, result);
            _creg.CalcZeroFlag(1, result);
            _creg.CalcAuxCarryFlag(0x01, dest);
            _creg.CalcParityFlag(result);
        }
        #endregion

        #region XCHG instructions
        private void ExecuteXCHG_AX()
        {
            // Op Codes: 90-97
            // note 90 is XCHG AX,AX and thus is a NOP

            // Parse the op code, last 3 bits indicates register.
            // All swaps are done with AX

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_currentOP, ref mod, ref reg, ref rm);

            int first = GetRegField16(rm);
            SaveRegField16(rm, _reg.AX);
            _reg.AX = (UInt16)first;

            // no flags are affected
        }

        private void ExecuteXCHG_General()
        {
            // Op Codes: 86-87
            // no flags

            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = GetDirection();
            int word_size = GetWordSize();

            int source = GetSourceData(direction, word_size, mod, reg, rm);
            int dest = GetDestinationData(direction, word_size, mod, reg, rm);

            SaveToDestination(source, direction, word_size, mod, reg, rm);

            // flip destination so this is saved to the original source
            direction ^= 1;

            SaveToDestination(dest, direction, word_size, mod, reg, rm);
        }
        #endregion

        #region MOV instructions
        private void ExecuteMOV_General()
        {
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = GetDirection();
            int word_size = GetWordSize();

            int source = GetSourceData(direction, word_size, mod, reg, rm);
            SaveToDestination(source, direction, word_size, mod, reg, rm);
        }
        private void ExecuteMOV_SReg()
        {
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(_bus.NextIP(), ref mod, ref reg, ref rm);

            int direction = GetDirection();
            int word_size = GetWordSize();

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
                        _reg.AL = _bus.GetData8(new DataRegister16(hi, lo));
                        break;
                    }
                case 0xa1:
                    {
                        _reg.AX = _bus.GetData16(new DataRegister16(hi, lo));
                        break;
                    }
                case 0xa2:
                    {
                        _bus.SaveData8(new DataRegister16(hi, lo), _reg.AL);
                        break;
                    }
                case 0xa3:
                    {
                        _bus.SaveData16(new DataRegister16(hi, lo), _reg.AX);
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

        #region Generic Retrieve and Store functions
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

        #region Interrupt Processing
        private void Interrupt(int int_type)
        {
            UInt16 int_ptr;

            // push flags
            Push(_creg.Register);

            // clear trap flag
            _creg.TrapFlag = false;

            // clear interrupt enable
            _creg.InterruptEnable = false;

            // push CS and IP
            Push(_bus.CS);
            Push(_bus.IP);

            // address of pointer is calculated by multiplying interrupt type by 4
            int_ptr = (UInt16)(int_type * 4);

            // the second word of the interrupt pointer replaces CS
            _bus.CS = _bus.GetData16(int_ptr + 2);

            // replace IP by first word of interrupt pointer
            _bus.CS = _bus.GetData16(int_ptr);
        }
        #endregion

        #region Operation Functions

        private void Execute_Increment(int word_size, byte mod, byte reg, byte rm)
        {
            int source = 1;
            int dest = GetDestinationData(0, word_size, mod, reg, rm);
            int result = dest + 1;

            SaveToDestination(result, 0, word_size, mod, reg, rm);

            // Flags: O S Z A P
            // Flags are set as if ADD or SUB instruction was used with operand2 = 1
            // Carry flag is not affected by increment
            _creg.CalcOverflowFlag(1, source, dest);
            _creg.CalcSignFlag(1, result);
            _creg.CalcZeroFlag(1, result);
            _creg.CalcAuxCarryFlag(source, dest);
            _creg.CalcParityFlag(result);
        }

        private void Execute_Decrement(int word_size, byte mod, byte reg, byte rm)
        {
            int source = 1;
            int dest = GetDestinationData(0, word_size, mod, reg, rm);
            int result = dest - 1;

            SaveToDestination(result, 0, word_size, mod, reg, rm);

            // Flags: O S Z A P
            // Flags are set as if ADD or SUB instruction was used with operand2 = 1
            // Carry flag is not affected by increment
            _creg.CalcOverflowSubtract(1, source, dest);
            _creg.CalcSignFlag(1, result);
            _creg.CalcZeroFlag(1, result);
            _creg.CalcAuxCarryFlag(source, dest);
            _creg.CalcParityFlag(result);
        }

        private void ADD_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool with_carry)
        {
            AssertMOD(mod);
            int result = 0;
            int offset;
            int dest = 0;
            int carry = 0;

            // Include carry flag if necessary
            if (with_carry && _creg.CarryFlag)
            {
                carry = 1;
            }

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (word_size == 0)
                {
                    dest = GetRegField8(reg);
                    result = source + dest + carry;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = source + dest + carry;
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
                            result = source + dest + carry;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = _bus.GetData(word_size, offset);
                            result = source + dest + carry;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0) 
                            {
                                dest = GetRegField8(rm);
                                result = source + dest + carry;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = source + dest + carry;
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
        }

        private void SUB_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool with_carry, bool comp_only)
        {
            AssertMOD(mod);
            int result = 0;
            int offset;
            int dest = 0;
            int carry = 0;

            // Include carry flag if necessary
            if (with_carry && _creg.CarryFlag)
            {
                carry = 1;
            }

            // if direction is 1 (R/M is source) action is the same regardless of mod
            if (direction == 1)
            {
                if (word_size == 0)
                {
                    dest = GetRegField8(reg);
                    result = dest - (source + carry);
                    if(!comp_only) SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest - (source + carry);
                    if (!comp_only) SaveRegField16(reg, (UInt16)result);
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
                            result = dest - (source + carry);
                            if (!comp_only) _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = _bus.GetData(word_size, offset);
                            result = dest - (source + carry);
                            if (!comp_only) _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest - (source + carry);
                                if (!comp_only) SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = dest - (source + carry);
                                if (!comp_only) SaveRegField16(rm, (UInt16)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            _creg.CalcOverflowSubtract(word_size, source + carry, dest);
            _creg.CalcSignFlag(word_size, result);
            _creg.CalcZeroFlag(word_size, result);
            _creg.CalcAuxCarryFlag(source, dest);
            _creg.CalcParityFlag(result);
            _creg.CalcCarryFlag(word_size, result);
        }

        private void AND_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool test_only)
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
                    result = dest & source;
                    if( !test_only ) SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest & source;
                    if (!test_only) SaveRegField16(reg, (UInt16)result);
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
                            result = dest & source;
                            if (!test_only) _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = _bus.GetData(word_size, offset);
                            result = dest & source;
                            if (!test_only) _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest & source;
                                if (!test_only) SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = dest & source;
                                if (!test_only) SaveRegField16(rm, (UInt16)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            _creg.OverflowFlag = false;
            _creg.CarryFlag = false;
            _creg.CalcSignFlag(word_size, result);
            _creg.CalcZeroFlag(word_size, result);
            _creg.CalcParityFlag(result);
        }

        private void OR_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm)
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
                    result = dest | source;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest | source;
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
                            result = dest | source;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = _bus.GetData(word_size, offset);
                            result = dest | source;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest | source;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = dest | source;
                                SaveRegField16(rm, (UInt16)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            _creg.OverflowFlag = false;
            _creg.CarryFlag = false;
            _creg.CalcSignFlag(word_size, result);
            _creg.CalcZeroFlag(word_size, result);
            _creg.CalcParityFlag(result);
        }

        private void XOR_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm)
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
                    result = dest ^ source;
                    SaveRegField8(reg, (byte)result);
                }
                else
                {
                    dest = GetRegField16(reg);
                    result = dest ^ source;
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
                            result = dest ^ source;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = _bus.GetData(word_size, offset);
                            result = dest ^ source;
                            _bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x03:
                        {
                            if (word_size == 0)
                            {
                                dest = GetRegField8(rm);
                                result = dest ^ source;
                                SaveRegField8(rm, (byte)result);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                dest = GetRegField16(rm);
                                result = dest ^ source;
                                SaveRegField16(rm, (UInt16)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            _creg.OverflowFlag = false;
            _creg.CarryFlag = false;
            _creg.CalcSignFlag(word_size, result);
            _creg.CalcZeroFlag(word_size, result);
            _creg.CalcParityFlag(result);
        }

        private void RotateLeft(int word_size, int source, byte mod, byte rm, bool through_carry, bool shift_only)
        {
            if (word_size == 0)
                RotateLeft8(source, mod, rm, through_carry, shift_only);
            else
                RotateLeft16(source, mod, rm, through_carry, shift_only);
        }

        private void RotateRight(int word_size, int source, byte mod, byte rm, bool through_carry, bool shift_only, bool arithmetic_shift)
        {
            if (word_size == 0)
                RotateRight8(source, mod, rm, through_carry, shift_only, arithmetic_shift);
            else
                RotateRight16(source, mod, rm, through_carry, shift_only, arithmetic_shift);
        }

        private void RotateLeft8(int source, byte mod, byte rm, bool through_carry, bool shift_only)
        {
            AssertMOD(mod);
            byte original = 0;
            byte result = 0;
            bool old_CF;
            int offset = 0;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = _bus.GetData8(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = _bus.GetData8(offset);
                        break;
                    }
                case 0x03:
                    {
                        original = GetRegField8(rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation or shift
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = _creg.CarryFlag;

                // carry bit equal to high bit
                _creg.CarryFlag = ((result & 0x80) == 0x80);

                // shift left
                result = (byte)((result << 1) & 0xff);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes low bit
                    // otherwise the original high bit (which is now the CF) becomes low bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (byte)(result | 0x01);
                    }
                    else
                    {
                        if (_creg.CarryFlag)
                            result = (byte)(result | 0x01);
                    }
                }
            }

            // save the result
            SaveToDestination(result, 0, 0, mod, 0, rm);

            // if the operand is 1 then the overflow flag is defined
            if( source == 1 )
            {
                // when shifting 1, if the two high order bits changed, set OF
                if( shift_only )
                {
                    _creg.OverflowFlag = ((original & 0xc0) != (result & 0xc0));
        }
                else
                {
                    // when rotating 1, if the sign changes as a result of the rotate, set OF
                    _creg.OverflowFlag = ((original ^ result) & 0x80) == 0x80;
                }
            }
        }

        private void RotateLeft16(int source, byte mod, byte rm, bool through_carry, bool shift_only)
        {
            AssertMOD(mod);
            int original = 0;
            int result = 0;
            int offset = 0;
            bool old_CF;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = _bus.GetData16(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = _bus.GetData16(offset);
                        break;
                    }
                case 0x03:
                    {
                        original = GetRegField16(rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = _creg.CarryFlag;

                // carry bit equal to high bit
                _creg.CarryFlag = ((result & 0x8000) == 0x8000);

                // shift left
                result = (UInt16)(result << 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes low bit
                    // otherwise the original high bit (which is now the CF) becomes low bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (UInt16)(result | 0x0001);
                    }
                    else
                    {
                        if (_creg.CarryFlag)
                            result = (UInt16)(result | 0x0001);
                    }
                }
            }

            // save result
            SaveToDestination(result, 0, 1, mod, 0, rm);

            // if the operand is 1 then the overflow flag is defined
            if (source == 1)
            {
                // when shifting 1, if the two high order bits changed, set OF
                if (shift_only)
                {
                    _creg.OverflowFlag = ((original & 0xc000) != (result & 0xc000));
                }
                else
                {
                    // when rotating 1, if the sign changes as a result of the rotate, set OF
                    _creg.OverflowFlag = ((original ^ result) & 0x8000) == 0x8000;
                }
            }
        }

        private void RotateRight8(int source, byte mod, byte rm, bool through_carry, bool shift_only, bool arithmetic_shift)
        {
            AssertMOD(mod);
            int original = 0;
            int result = 0;
            int offset = 0;
            bool old_CF = false;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = _bus.GetData8(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = _bus.GetData8(offset) ;
                        break;
                    }
                case 0x03:
                    {
                        original = GetRegField8(rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = _creg.CarryFlag;

                // carry bit equal to low bit
                _creg.CarryFlag = ((result & 0x01) == 0x01);

                // shift right
                result = (byte)(result >> 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes high bit
                    // otherwise the original low bit (which is now the CF) becomes high bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (byte)(result | 0x80);
                    }
                    else
                    {
                        if (_creg.CarryFlag)
                            result = (byte)(result | 0x80);
                    }
                }

                // if arithmetic shift then the sign bit retains its original value
                // set SF, ZF, PF
                if( arithmetic_shift )
                {
                    result = (byte)(result | (byte)(original & 0x80));
                    _creg.CalcSignFlag(0, result);
                    _creg.CalcZeroFlag(0, result);
                    _creg.CalcParityFlag(result);
                }
            }

            // save the result
            SaveToDestination(result, 0, 0, mod, 0, rm);

            // overflow flag - only calculated if count is 1
            if( source == 1 )
            {
                // arithmetic shift always clears OF
                if(arithmetic_shift)
                {
                    _creg.OverflowFlag = false;
                }
                // if shift, set OF if the sign has changed
                else if (shift_only)
                {
                    _creg.OverflowFlag = ((original ^ result) & 0x80) == 0x80;
                }
                // if rotate through carry, set OF if high order bit and carry flags have changed
                else if( !shift_only & through_carry )
                {
                    _creg.OverflowFlag = (((original ^ result) & 0x80) == 0x80) & (old_CF == _creg.CarryFlag);
                }
            }
        }

        private void RotateRight16(int source, byte mod, byte rm, bool through_carry, bool shift_only, bool arithmetic_shift)
        {
            AssertMOD(mod);
            int original = 0;
            int result = 0;
            int offset = 0;
            bool old_CF = false;
            switch (mod)
            {
                case 0x00:
                    {
                        offset = GetRMTable1(rm);
                        original = _bus.GetData16(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = _bus.GetData16(offset);
                        break;
                    }
                case 0x03:
                    {
                        original = GetRegField16(rm);
                        break;
                    }
            }

            // preserve the original value
            result = original;

            // perform the rotation
            for (int ii = 1; ii <= source; ii++)
            {
                // if through carry, then original CF value becomes low bit
                old_CF = _creg.CarryFlag;

                // carry bit equal to low bit
                _creg.CarryFlag = ((result & 0x0001) == 0x0001);

                // shift right
                result = (UInt16)(result >> 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes high bit
                    // otherwise the original low bit (which is now the CF) becomes high bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (UInt16)(result | 0x8000);
                    }
                    else
                    {
                        if (_creg.CarryFlag)
                            result = (UInt16)(result | 0x8000);
                    }
                }

                // if arithmetic shift then the sign bit retains its original value
                if (arithmetic_shift)
                {
                    result = (byte)(result | (byte)(original & 0x80));
                    _creg.CalcSignFlag(1, result);
                    _creg.CalcZeroFlag(1, result);
                    _creg.CalcParityFlag(result);
                }
            }

            // save the result
            SaveToDestination(result, 0, 1, mod, 0, rm);

            // overflow flag - only calculated if count is 1
            if (source == 1)
            {
                // arithmetic shift always clears OF
                if (arithmetic_shift)
                {
                    _creg.OverflowFlag = false;
                }
                // if shift, set OF if the sign has changed
                else if (shift_only)
                {
                    _creg.OverflowFlag = ((original ^ result) & 0x8000) == 0x8000;
                }
                // if rotate through carry, set OF if high order bit and carry flags have changed
                else if (!shift_only & through_carry)
                {
                    _creg.OverflowFlag = (((original ^ result) & 0x8000) == 0x8000) & (old_CF == _creg.CarryFlag);
                }
            }
        }

        private void Push(UInt16 value)
        {
            _reg.SP -= 2;
            _bus.PushStack(_reg.SP, value);
        }

        private UInt16 Pop()
        {
            UInt16 result = _bus.PopStack(_reg.SP);
            _reg.SP += 2;
            return result;
        }

        #endregion

        #region Get and Set registers
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
                case 0x03:
                    {
                        _bus.DS = value;
                        break;
                    }
            }
        }
        #endregion

        #region R/M Tables
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
                        // There is only one RM table lookup per instruction but this may be invoked more than once
                        // for certain instructions.  For this reason we preserve the offset in case it is needed again.
                        if (_RMTableLookupCount == _instrCount)
                        {
                            result = _RMTableLastLookup;
                        }
                        else
                        {
                            result = GetImmediate16();
                            _RMTableLastLookup = result;
                            _RMTableLookupCount = _instrCount;
                            
                        }
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
                // There is only one RM table lookup per instruction but this may be invoked more than once
                // for certain instructions.  For this reason we preserve the offset in case it is needed again.
                case 0x01:
                    {
                        // 8 bit displacement
                        if (_RMTableLookupCount == _instrCount)
                        {
                            disp = _RMTableLastLookup;
                        }
                        else
                        {
                            disp = _bus.NextIP();
                            _RMTableLastLookup = disp;
                            _RMTableLookupCount = _instrCount;
                        }
                        break;
                    }
                case 0x02:
                    {
                        // 16 bit displacement
                        if (_RMTableLookupCount == _instrCount)
                        {
                            disp = _RMTableLastLookup;
                        }
                        else
                        {
                            disp = GetImmediate16();
                            _RMTableLastLookup = disp;
                            _RMTableLookupCount = _instrCount;
                        }
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

        #region Helper and Utility functions

        // Gets the immediate 16 bit value
        private UInt16 GetImmediate16()
        {
            byte lo = _bus.NextIP();
            byte hi = _bus.NextIP();
            return new DataRegister16(hi, lo);
        }

        private void SplitAddrByte(byte addr, ref byte mod, ref byte reg, ref byte rm)
        {
            mod = (byte)((addr >> 6) & 0x03);
            reg = (byte)((addr >> 3) & 0x07);
            rm = (byte)(addr & 0x07);
        }

        private int GetDirection()
        {
            return (_currentOP >> 1) & 0x01;
        }

        private int GetWordSize()
        {
            return (_currentOP & 0x01);
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

        // Sign extend 8 bits to 16 bits
        private UInt16 SignExtend(byte num)
        {
            if (num < 0x80)
                return num;
            else
                return new DataRegister16(0xff, num);
        }

        // Sign extend 16 bits to 32 bits
        private UInt32 SignExtend32(UInt16 num)
        {
            if (num < 0x8000)
                return num;
            else
                return num | 0xffffff00;
        }

        #endregion  
    }
}
