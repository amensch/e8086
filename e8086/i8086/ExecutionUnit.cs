﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class ExecutionUnit : IExecutionUnit
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
        public Statistics Stats { get; private set; } = new Statistics();
        public long InstructionCount { get; set; } = 0;
        private long _RMTableLookupCount = 0;
        private ushort _RMTableLastLookup = 0;

        // Preserve the current OP code
        private byte _currentOP;

        // Table of OpCodes
        private OpCodeTable _opTable = new OpCodeTable();
        private Dictionary<int, Instruction> instructions = new Dictionary<int, Instruction>();

        // General Registers: AX, BX, CX, DX and SP, BP, SI, DI
        public Registers Registers { get; private set; } = new Registers();

        // Flags
        public ConditionalRegister CondReg { get; private set; } = new ConditionalRegister();

        // Bus Interface Unit
        public IBus Bus { get; private set; }

        // I/O Ports
        private Dictionary<int, IInputDevice> _inputDevices;
        private Dictionary<int, IOutputDevice> _outputDevices;

        // Repeat flag
        private bool _repeat = false;
        private int _repeatType = 0;

        // Property to indicate a halt has been encountered
        public bool Halted { get; set; }

        public ExecutionUnit(IBus bus)
        {
            Bus = bus;
            Halted = false;

            _inputDevices = new Dictionary<int, IInputDevice>();
            _outputDevices = new Dictionary<int, IOutputDevice>();

            InitOpCodeTable();
        }

        public void Tick()
        {

            // If the trap flag is set, trigger interrupt 1
            if (CondReg.TrapFlag)
            {
                Interrupt(1);
            }

            // If interrupts are enabled and there is an interrupt waiting,
            // trigger the next interrupt.  Only one interrupt is processed per
            // tick if multiple are waiting.
            if( CondReg.InterruptEnable ) // && i8259 has an interrupt waiting
            {
                // Interrupt( i8259 next interrupt ) ;
            }

            // Retrieve the next instruction and count stats
            InstructionCount++;
            _currentOP = Bus.NextIP();
            Stats.AddOpCode(_currentOP);

            // If segment override then process that right here.
            // Process the next instruction immediately after.  Cannot allow an interrupt here or
            // the segment override will be lost after the interrupt.
            if (_currentOP == 0x26)
            {
                Bus.SegmentOverride = SegmentOverrideState.UseES;
                InstructionCount++;
                _currentOP = Bus.NextIP();
                Stats.AddOpCode(_currentOP);
            }
            else if (_currentOP == 0x2e)
            {
                Bus.SegmentOverride = SegmentOverrideState.UseCS;
                InstructionCount++;
                _currentOP = Bus.NextIP();
                Stats.AddOpCode(_currentOP);
            }
            else if (_currentOP == 0x36)
            {
                Bus.SegmentOverride = SegmentOverrideState.UseSS;
                InstructionCount++;
                _currentOP = Bus.NextIP();
                Stats.AddOpCode(_currentOP);
            }
            else if (_currentOP == 0x3e)
            {
                Bus.SegmentOverride = SegmentOverrideState.UseDS;
                InstructionCount++;
                _currentOP = Bus.NextIP();
                Stats.AddOpCode(_currentOP);
            }

            // If this is in the dictionary of op codes call it, otherwise
            // use the "old" array
            if (instructions.ContainsKey(_currentOP))
            {
                instructions[_currentOP].Execute();
            }
            else
            {

                // Call method to execute this instruction
                _opTable[_currentOP].opAction();
            }

            // NOTE: a current minor flaw here is if there is a repeat instruction because the entire loop
            // will get executed immediately without allowing for any interrupts.  Watch for timing issues
            // with PIT if there is a long loop.

            // Tick the PIT
            // i8253.Tick()

            // After executing the instruction reset the override and base pointer flags.
            Bus.SegmentOverride = SegmentOverrideState.NoOverride;
            Bus.UsingBasePointer = false;
            _repeat = false;
        }

        public void AddInputDevice(int port, IInputDevice device)
        {
            if (_inputDevices.ContainsKey(port))
            {
                _inputDevices.Remove(port);
            }
            _inputDevices.Add(port, device);
        }

        public void AddOutputDevice(int port, IOutputDevice device)
        {
            if (_outputDevices.ContainsKey(port))
            {
                _outputDevices.Remove(port);
            }
            _outputDevices.Add(port, device);
        }

        private void InitOpCodeTable()
        {
            instructions.Add(0x00, new ADD(0x00, this, Bus));
            instructions.Add(0x01, new ADD(0x01, this, Bus));
            instructions.Add(0x02, new ADD(0x02, this, Bus));
            instructions.Add(0x03, new ADD(0x03, this, Bus));
            instructions.Add(0x04, new ADD_Immediate(0x04, this, Bus));
            instructions.Add(0x05, new ADD_Immediate(0x05, this, Bus));
            instructions.Add(0x06, new PUSH(0x06, this, Bus));
            instructions.Add(0x07, new POP(0x07, this, Bus));
            instructions.Add(0x08, new OR(0x08, this, Bus));
            instructions.Add(0x09, new OR(0x09, this, Bus));
            instructions.Add(0x0a, new OR(0x0a, this, Bus));
            instructions.Add(0x0b, new OR(0x0b, this, Bus));
            instructions.Add(0x0c, new OR_Immediate(0x0c, this, Bus));
            instructions.Add(0x0d, new OR_Immediate(0x0d, this, Bus));
            instructions.Add(0x0e, new PUSH(0x0e, this, Bus));
            instructions.Add(0x0f, new NOOP(0x0f, this, Bus)); // POP CS is not a valid instruction
            instructions.Add(0x10, new ADC(0x10, this, Bus));
            instructions.Add(0x11, new ADC(0x11, this, Bus));
            instructions.Add(0x12, new ADC(0x12, this, Bus));
            instructions.Add(0x13, new ADC(0x13, this, Bus));
            instructions.Add(0x14, new ADC_Immediate(0x14, this, Bus));
            instructions.Add(0x15, new ADC_Immediate(0x15, this, Bus));
            instructions.Add(0x16, new PUSH(0x16, this, Bus));
            instructions.Add(0x17, new POP(0x17, this, Bus));
            instructions.Add(0x18, new SBB(0x18, this, Bus));
            instructions.Add(0x19, new SBB(0x19, this, Bus));
            instructions.Add(0x1a, new SBB(0x1a, this, Bus));
            instructions.Add(0x1b, new SBB(0x1b, this, Bus));
            instructions.Add(0x1c, new SBB_Immediate(0x1c, this, Bus));
            instructions.Add(0x1d, new SBB_Immediate(0x1d, this, Bus));
            instructions.Add(0x1e, new PUSH(0x1e, this, Bus));
            instructions.Add(0x1f, new POP(0x1f, this, Bus));
            instructions.Add(0x20, new AND(0x20, this, Bus));
            instructions.Add(0x21, new AND(0x21, this, Bus));
            instructions.Add(0x22, new AND(0x22, this, Bus));
            instructions.Add(0x23, new AND(0x23, this, Bus));
            instructions.Add(0x24, new AND_Immediate(0x24, this, Bus));
            instructions.Add(0x25, new AND_Immediate(0x25, this, Bus));
            // 0x26: segment override instruction
            instructions.Add(0x27, new DAA(0x27, this, Bus));
            instructions.Add(0x28, new SUB(0x28, this, Bus));
            instructions.Add(0x29, new SUB(0x29, this, Bus));
            instructions.Add(0x2a, new SUB(0x2a, this, Bus));
            instructions.Add(0x2b, new SUB(0x2b, this, Bus));
            instructions.Add(0x2c, new SUB_Immediate(0x2c, this, Bus));
            instructions.Add(0x2d, new SUB_Immediate(0x2d, this, Bus));
            // 0x2e: segment override instruction
            instructions.Add(0x2f, new DAS(0x2f, this, Bus));
            instructions.Add(0x30, new XOR(0x30, this, Bus));
            instructions.Add(0x31, new XOR(0x31, this, Bus));
            instructions.Add(0x32, new XOR(0x32, this, Bus));
            instructions.Add(0x33, new XOR(0x33, this, Bus));
            instructions.Add(0x34, new XOR_Immediate(0x34, this, Bus));
            instructions.Add(0x35, new XOR_Immediate(0x35, this, Bus));
            // 0x36: segment override instruction
            instructions.Add(0x37, new AAA(0x37, this, Bus));
            instructions.Add(0x38, new CMP(0x38, this, Bus));
            instructions.Add(0x39, new CMP(0x39, this, Bus));
            instructions.Add(0x3a, new CMP(0x3a, this, Bus));
            instructions.Add(0x3b, new CMP(0x3b, this, Bus));
            instructions.Add(0x3c, new CMP_Immediate(0x3c, this, Bus));
            instructions.Add(0x3d, new CMP_Immediate(0x3d, this, Bus));
            // 0x3e, segment override instruction
            instructions.Add(0x3f, new AAS(0x3f, this, Bus));
            instructions.Add(0x40, new INC(0x40, this, Bus));
            instructions.Add(0x41, new INC(0x41, this, Bus));
            instructions.Add(0x42, new INC(0x42, this, Bus));
            instructions.Add(0x43, new INC(0x43, this, Bus));
            instructions.Add(0x44, new INC(0x44, this, Bus));
            instructions.Add(0x45, new INC(0x45, this, Bus));
            instructions.Add(0x46, new INC(0x46, this, Bus));
            instructions.Add(0x47, new INC(0x47, this, Bus));
            instructions.Add(0x48, new DEC(0x48, this, Bus));
            instructions.Add(0x49, new DEC(0x49, this, Bus));
            instructions.Add(0x4a, new DEC(0x4a, this, Bus));
            instructions.Add(0x4b, new DEC(0x4b, this, Bus));
            instructions.Add(0x4c, new DEC(0x4c, this, Bus));
            instructions.Add(0x4d, new DEC(0x4d, this, Bus));
            instructions.Add(0x4e, new DEC(0x4e, this, Bus));
            instructions.Add(0x4f, new DEC(0x4f, this, Bus));
            instructions.Add(0x50, new PUSH(0x50, this, Bus));
            instructions.Add(0x51, new PUSH(0x51, this, Bus));
            instructions.Add(0x52, new PUSH(0x52, this, Bus));
            instructions.Add(0x53, new PUSH(0x53, this, Bus));
            instructions.Add(0x54, new PUSH(0x54, this, Bus));
            instructions.Add(0x55, new PUSH(0x55, this, Bus));
            instructions.Add(0x56, new PUSH(0x56, this, Bus));
            instructions.Add(0x57, new PUSH(0x57, this, Bus));
            instructions.Add(0x58, new POP(0x58, this, Bus));
            instructions.Add(0x59, new POP(0x59, this, Bus));
            instructions.Add(0x5a, new POP(0x5a, this, Bus));
            instructions.Add(0x5b, new POP(0x5b, this, Bus));
            instructions.Add(0x5c, new POP(0x5c, this, Bus));
            instructions.Add(0x5d, new POP(0x5d, this, Bus));
            instructions.Add(0x5e, new POP(0x5e, this, Bus));
            instructions.Add(0x5f, new POP(0x5f, this, Bus));
            instructions.Add(0x60, new PUSHA(0x60, this, Bus));
            instructions.Add(0x61, new POPA(0x61, this, Bus));
            instructions.Add(0x62, new NOOP_TwoByte(0x62, this, Bus));  // TODO (Bound Instruction)
            instructions.Add(0x63, new InvalidInstruction(0x63, this, Bus));
            instructions.Add(0x64, new InvalidInstruction(0x64, this, Bus));
            instructions.Add(0x65, new InvalidInstruction(0x65, this, Bus));
            instructions.Add(0x66, new InvalidInstruction(0x66, this, Bus));
            instructions.Add(0x67, new InvalidInstruction(0x67, this, Bus));
            instructions.Add(0x68, new PUSH_ImmediateWord(0x68, this, Bus));
            instructions.Add(0x69, new IMUL_ImmediateWord(0x69, this, Bus));
            instructions.Add(0x6a, new PUSH_ImmediateByte(0x6a, this, Bus));
            instructions.Add(0x6b, new IMUL_ImmediateByte(0x6b, this, Bus));

            instructions.Add(0x70, new JO(0x70, this, Bus));
            instructions.Add(0x71, new JNO(0x71, this, Bus));
            instructions.Add(0x72, new JC(0x72, this, Bus));
            instructions.Add(0x73, new JNC(0x73, this, Bus));
            instructions.Add(0x74, new JZ(0x74, this, Bus));
            instructions.Add(0x75, new JNZ(0x75, this, Bus));
            instructions.Add(0x76, new JNA(0x76, this, Bus));
            instructions.Add(0x77, new JA(0x77, this, Bus));
            instructions.Add(0x78, new JS(0x78, this, Bus));
            instructions.Add(0x79, new JNS(0x79, this, Bus));
            instructions.Add(0x7a, new JP(0x7a, this, Bus));
            instructions.Add(0x7b, new JNP(0x7b, this, Bus));
            instructions.Add(0x7c, new JL(0x7c, this, Bus));
            instructions.Add(0x7d, new JNL(0x7d, this, Bus));
            instructions.Add(0x7e, new JNG(0x7e, this, Bus));
            instructions.Add(0x7f, new JG(0x7f, this, Bus));
            instructions.Add(0x80, new GRP1(0x80, this, Bus));
            instructions.Add(0x81, new GRP1(0x81, this, Bus));
            instructions.Add(0x82, new GRP1(0x82, this, Bus));
            instructions.Add(0x83, new GRP1(0x83, this, Bus));
            instructions.Add(0x84, new TEST(0x84, this, Bus));
            instructions.Add(0x85, new TEST(0x85, this, Bus));
            instructions.Add(0x86, new XCHG(0x86, this, Bus));
            instructions.Add(0x87, new XCHG(0x87, this, Bus));
            instructions.Add(0x88, new MOV(0x88, this, Bus));
            instructions.Add(0x89, new MOV(0x89, this, Bus));
            instructions.Add(0x8a, new MOV(0x8a, this, Bus));
            instructions.Add(0x8b, new MOV(0x8b, this, Bus));
            instructions.Add(0x8c, new MOV_SReg(0x8c, this, Bus));
            instructions.Add(0x8d, new LEA(0x8d, this, Bus));
            instructions.Add(0x8e, new MOV_SReg(0x8e, this, Bus));
            instructions.Add(0x8f, new POP_regmem(0x8f, this, Bus));
            instructions.Add(0x90, new XCHG_AX(0x90, this, Bus));
            instructions.Add(0x91, new XCHG_AX(0x91, this, Bus));
            instructions.Add(0x92, new XCHG_AX(0x92, this, Bus));
            instructions.Add(0x93, new XCHG_AX(0x93, this, Bus));
            instructions.Add(0x94, new XCHG_AX(0x94, this, Bus));
            instructions.Add(0x95, new XCHG_AX(0x95, this, Bus));
            instructions.Add(0x96, new XCHG_AX(0x96, this, Bus));
            instructions.Add(0x97, new XCHG_AX(0x97, this, Bus));
            instructions.Add(0x98, new CBW(0x98, this, Bus));
            instructions.Add(0x99, new CWD(0x99, this, Bus));
            instructions.Add(0x9a, new CALL_Far(0x9a, this, Bus));
            instructions.Add(0x9b, new NOOP(0x9b, this, Bus));  // TODO: WAIT instruction
            instructions.Add(0x9c, new PUSHF(0x9c, this, Bus));
            instructions.Add(0x9d, new POPF(0x9d, this, Bus));
            instructions.Add(0x9e, new SAHF(0x9e, this, Bus));
            instructions.Add(0x9f, new LAHF(0x9f, this, Bus));
            instructions.Add(0xa0, new MOV_MemImmediate(0xa0, this, Bus));
            instructions.Add(0xa1, new MOV_MemImmediate(0xa1, this, Bus));
            instructions.Add(0xa2, new MOV_MemImmediate(0xa2, this, Bus));
            instructions.Add(0xa3, new MOV_MemImmediate(0xa3, this, Bus));

            instructions.Add(0xa8, new TEST_Immediate(0xa8, this, Bus));
            instructions.Add(0xa9, new TEST_Immediate(0xa9, this, Bus));

            instructions.Add(0xb0, new MOV_ImmediateByte(0xb0, this, Bus));
            instructions.Add(0xb1, new MOV_ImmediateByte(0xb1, this, Bus));
            instructions.Add(0xb2, new MOV_ImmediateByte(0xb2, this, Bus));
            instructions.Add(0xb3, new MOV_ImmediateByte(0xb3, this, Bus));
            instructions.Add(0xb4, new MOV_ImmediateByte(0xb4, this, Bus));
            instructions.Add(0xb5, new MOV_ImmediateByte(0xb5, this, Bus));
            instructions.Add(0xb6, new MOV_ImmediateByte(0xb6, this, Bus));
            instructions.Add(0xb7, new MOV_ImmediateByte(0xb7, this, Bus));
            instructions.Add(0xb8, new MOV_ImmediateWord(0xb8, this, Bus));
            instructions.Add(0xb9, new MOV_ImmediateWord(0xb9, this, Bus));
            instructions.Add(0xba, new MOV_ImmediateWord(0xba, this, Bus));
            instructions.Add(0xbb, new MOV_ImmediateWord(0xbb, this, Bus));
            instructions.Add(0xbc, new MOV_ImmediateWord(0xbc, this, Bus));
            instructions.Add(0xbd, new MOV_ImmediateWord(0xbd, this, Bus));
            instructions.Add(0xbe, new MOV_ImmediateWord(0xbe, this, Bus));
            instructions.Add(0xbf, new MOV_ImmediateWord(0xbf, this, Bus));

            // GRP2 rm-8 imm-8 (80186)
            instructions.Add(0xc0, new RotateAndShiftImmediate(0xc0, this, Bus));
            // GRP2 rm-16 imm-16 (80186)
            instructions.Add(0xc1, new RotateAndShiftImmediate(0xc1, this, Bus));

            instructions.Add(0xd0, new RotateAndShift(0xd0, this, Bus));
            instructions.Add(0xd1, new RotateAndShift(0xd1, this, Bus));
            instructions.Add(0xd2, new RotateAndShiftCL(0xd2, this, Bus));
            instructions.Add(0xd3, new RotateAndShiftCL(0xd3, this, Bus));


            instructions.Add(0xd6, new SALC(0xd6, this, Bus));
            instructions.Add(0xd7, new XLAT(0xd7, this, Bus));
            // D8-DF ESC OPCODE,SOURCE -- math coprocessor instructions
            instructions.Add(0xd8, new NOOP_TwoByte(0xd8, this, Bus));
            instructions.Add(0xd9, new NOOP_TwoByte(0xd9, this, Bus));
            instructions.Add(0xda, new NOOP_TwoByte(0xda, this, Bus));
            instructions.Add(0xdb, new NOOP_TwoByte(0xdb, this, Bus));
            instructions.Add(0xdc, new NOOP_TwoByte(0xdc, this, Bus));
            instructions.Add(0xdd, new NOOP_TwoByte(0xdd, this, Bus));
            instructions.Add(0xde, new NOOP_TwoByte(0xde, this, Bus));
            instructions.Add(0xdf, new NOOP_TwoByte(0xdf, this, Bus));

            instructions.Add(0xe3, new JCXZ(0xe3, this, Bus));

            instructions.Add(0xe8, new CALL_Near(0xe8, this, Bus));
            instructions.Add(0xe9, new JMP_Near(0xe9, this, Bus));
            instructions.Add(0xea, new JMP_Far(0xea, this, Bus));
            instructions.Add(0xeb, new JMP(0xeb, this, Bus));

            instructions.Add(0xf5, new CMC(0xf5, this, Bus));

            instructions.Add(0xf8, new CLC(0xf8, this, Bus));
            instructions.Add(0xf9, new STC(0xf9, this, Bus));
            instructions.Add(0xfa, new CLI(0xfa, this, Bus));
            instructions.Add(0xfb, new STI(0xfb, this, Bus));
            instructions.Add(0xfc, new CLD(0xfc, this, Bus));
            instructions.Add(0xfd, new STD(0xfd, this, Bus));

            //_opTable[0x00] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x01] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x02] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x03] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x04] = new OpCodeRecord(ExecuteADD_Immediate);
            //_opTable[0x05] = new OpCodeRecord(ExecuteADD_Immediate);
            //_opTable[0x06] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x07] = new OpCodeRecord(Execute_POP);
            //_opTable[0x08] = new OpCodeRecord(ExecuteLogical_General);      // OR 
            //_opTable[0x09] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x0a] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x0b] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x0c] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x0d] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x0e] = new OpCodeRecord(Execute_PUSH);
            // POP CS is not a valid instruction
            //_opTable[0x0f] = new OpCodeRecord(() => { });
            //_opTable[0x10] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x11] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x12] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x13] = new OpCodeRecord(ExecuteADD_General);
            //_opTable[0x14] = new OpCodeRecord(ExecuteADD_Immediate);
            //_opTable[0x15] = new OpCodeRecord(ExecuteADD_Immediate);
            //_opTable[0x16] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x17] = new OpCodeRecord(Execute_POP);
            //_opTable[0x18] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x19] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x1a] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x1b] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x1c] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x1d] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x1e] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x1f] = new OpCodeRecord(Execute_POP);  
            //_opTable[0x20] = new OpCodeRecord(ExecuteLogical_General);     // AND
            //_opTable[0x21] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x22] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x23] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x24] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x25] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x26]  segment override is processed in the NextInstruction() method
            //_opTable[0x27] = new OpCodeRecord(Execute_DecimalAdjustADD);
            //_opTable[0x28] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x29] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x2a] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x2b] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x2c] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x2d] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x2e]  segment override is processed in the NextInstruction() method
            //_opTable[0x2f] = new OpCodeRecord(Execute_DecimalAdjustSUB);
            //_opTable[0x30] = new OpCodeRecord(ExecuteLogical_General);  // XOR
            //_opTable[0x31] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x32] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x33] = new OpCodeRecord(ExecuteLogical_General);
            //_opTable[0x34] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x35] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0x36]  segment override is processed in the NextInstruction() method
            //_opTable[0x37] = new OpCodeRecord(Execute_AsciiAdjustADD);
            //_opTable[0x38] = new OpCodeRecord(ExecuteSUB_General);       // CMP 
            //_opTable[0x39] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x3a] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x3b] = new OpCodeRecord(ExecuteSUB_General);
            //_opTable[0x3c] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x3d] = new OpCodeRecord(ExecuteSUB_Immediate);
            //_opTable[0x3e]  segment override is processed in the NextInstruction() method
            //_opTable[0x3f] = new OpCodeRecord(Execute_AsciiAdjustSUB);
            //_opTable[0x40] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x41] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x42] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x43] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x44] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x45] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x46] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x47] = new OpCodeRecord(ExecuteINC);
            //_opTable[0x48] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x49] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x4a] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x4b] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x4c] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x4d] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x4e] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x4f] = new OpCodeRecord(ExecuteDEC);
            //_opTable[0x50] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x51] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x52] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x53] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x54] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x55] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x56] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x57] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x58] = new OpCodeRecord(Execute_POP);
            //_opTable[0x59] = new OpCodeRecord(Execute_POP);
            //_opTable[0x5a] = new OpCodeRecord(Execute_POP);
            //_opTable[0x5b] = new OpCodeRecord(Execute_POP);
            //_opTable[0x5c] = new OpCodeRecord(Execute_POP);
            //_opTable[0x5d] = new OpCodeRecord(Execute_POP);
            //_opTable[0x5e] = new OpCodeRecord(Execute_POP);
            //_opTable[0x5f] = new OpCodeRecord(Execute_POP);
            //_opTable[0x60] = new OpCodeRecord(Execute_PUSHA);
            //_opTable[0x61] = new OpCodeRecord(Execute_POPA);
            //_opTable[0x62] = new OpCodeRecord(Execute_Bound);
            //_opTable[0x63] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x63 is not implemented"); });
            //_opTable[0x64] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x64 is not implemented"); });
            //_opTable[0x65] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x65 is not implemented"); });
            //_opTable[0x66] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x66 is not implemented"); });
            //_opTable[0x67] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0x67 is not implemented"); });
            //_opTable[0x68] = new OpCodeRecord(() =>  // PUSH imm-16
            //{
            //    Push(GetImmediate16());
            //});
            //_opTable[0x69] = new OpCodeRecord(() => // IMUL REG-16, RM-16, Imm
            //{
            //    byte mod = 0, reg = 0, rm = 0;
            //    SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

            //    int word_size = GetWordSize();
            //    int direction = GetDirection();

            //    ushort oper1 = (ushort)GetSourceData(direction, word_size, mod, reg, rm);
            //    ushort oper2 = GetImmediate16();

            //    uint oper1ext = SignExtend32(oper1);
            //    uint oper2ext = SignExtend32(oper2);

            //    uint result = oper1ext * oper2ext;

            //    SaveToDestination((ushort)(result & 0xffff), direction, word_size, mod, reg, rm);

            //    if( (result & 0xffff0000) != 0 )
            //    {
            //        CondReg.CarryFlag = true;
            //        CondReg.OverflowFlag = true;
            //    }
            //    else
            //    {
            //        CondReg.CarryFlag = false;
            //        CondReg.OverflowFlag = false;
            //    }

            //});
            //_opTable[0x6a] = new OpCodeRecord(() =>  // PUSH imm-8
            //{
            //    Push(Bus.NextIP());
            //});
            //_opTable[0x6b] = new OpCodeRecord(() => 
            //{
            //    byte mod = 0, reg = 0, rm = 0;
            //    SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

            //    int word_size = GetWordSize();
            //    int direction = GetDirection();

            //    ushort oper1 = (ushort)GetSourceData(direction, word_size, mod, reg, rm);
            //    ushort oper2 = Bus.NextIP();

            //    uint oper1ext = SignExtend32(oper1);
            //    uint oper2ext = SignExtend32(oper2);

            //    uint result = oper1ext * oper2ext;

            //    SaveToDestination((ushort)(result & 0xffff), direction, word_size, mod, reg, rm);

            //    if ((result & 0xffff0000) != 0)
            //    {
            //        CondReg.CarryFlag = true;
            //        CondReg.OverflowFlag = true;
            //    }
            //    else
            //    {
            //        CondReg.CarryFlag = false;
            //        CondReg.OverflowFlag = false;
            //    }
            //});

            _opTable[0x6c] = new OpCodeRecord(() => // INSB
            {
                // if repetition is on but CX is 0 then do nothing
                if( ( _repeat ) && ( Registers.CX == 0 ) )
                {
                }
                else
                {
                    do
                    {
                        Execute_IN_String();

                        if (CondReg.DirectionFlag)
                            Registers.DI--;
                        else
                            Registers.DI++;

                        if (_repeat)
                            Registers.CX--;
                    } while (Registers.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x6d] = new OpCodeRecord(() => // INSW
            {
                // if repetition is on but CX is 0 then do nothing
                if ((_repeat) && (Registers.CX == 0))
                {
                }
                else
                {
                    do
                    {
                        Execute_IN_String();

                            if (CondReg.DirectionFlag)
                                Registers.DI -= 2;
                            else
                                Registers.DI += 2;

                        if (_repeat)
                            Registers.CX--;
                    } while (Registers.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x6e] = new OpCodeRecord(() => // OUTSB
            {
                // if repetition is on but CX is 0 then do nothing
                if ((_repeat) && (Registers.CX == 0))
                {
                }
                else
                {
                    do
                    {
                        Execute_OUT_String();

                        if (CondReg.DirectionFlag)
                            Registers.SI -= 1;
                        else
                            Registers.SI += 1;

                        if (_repeat)
                            Registers.CX--;
                    } while (Registers.CX != 0);
                    _repeat = false;
                }
            });
            _opTable[0x6f] = new OpCodeRecord(() => // OUTSW
            {
                // if repetition is on but CX is 0 then do nothing
                if ((_repeat) && (Registers.CX == 0))
                {
                }
                else
                {
                    do
                    {
                        Execute_OUT_String();

                        if (CondReg.DirectionFlag)
                            Registers.SI -= 2;
                        else
                            Registers.SI += 2;

                        if (_repeat)
                            Registers.CX--;
                    } while (Registers.CX != 0);
                    _repeat = false;
                }
            });
            //_opTable[0x70] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x71] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x72] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x73] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x74] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x75] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x76] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x77] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x78] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x79] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x7a] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x7b] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x7c] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x7d] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x7e] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x7f] = new OpCodeRecord(Execute_CondJump);
            //_opTable[0x80] = new OpCodeRecord(Execute_Group1);
            //_opTable[0x81] = new OpCodeRecord(Execute_Group1);
            //_opTable[0x82] = new OpCodeRecord(Execute_Group1);
            //_opTable[0x83] = new OpCodeRecord(Execute_Group1);
            //_opTable[0x84] = new OpCodeRecord(ExecuteLogical_General);  // TEST
            //_opTable[0x85] = new OpCodeRecord(ExecuteLogical_General);  // TEST
            //_opTable[0x86] = new OpCodeRecord(ExecuteXCHG_General);
            //_opTable[0x87] = new OpCodeRecord(ExecuteXCHG_General);
            //_opTable[0x88] = new OpCodeRecord(ExecuteMOV_General);
            //_opTable[0x89] = new OpCodeRecord(ExecuteMOV_General);
            //_opTable[0x8a] = new OpCodeRecord(ExecuteMOV_General);
            //_opTable[0x8b] = new OpCodeRecord(ExecuteMOV_General);
            //_opTable[0x8c] = new OpCodeRecord(ExecuteMOV_SReg);
            //_opTable[0x8d] = new OpCodeRecord(Execute_LEA);
            //_opTable[0x8e] = new OpCodeRecord(ExecuteMOV_SReg);  // MOV CS - this should never be used in practice
            //_opTable[0x8f] = new OpCodeRecord(Execute_POP);
            //_opTable[0x90] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x91] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x92] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x93] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x94] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x95] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x96] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x97] = new OpCodeRecord(ExecuteXCHG_AX);
            //_opTable[0x98] = new OpCodeRecord(Execute_CBW);
            //_opTable[0x99] = new OpCodeRecord(Execute_CWD);
            //_opTable[0x9a] = new OpCodeRecord(Execute_CallFar);
            //_opTable[0x9b] = new OpCodeRecord(() => { }); // WAIT (for now NOP)
            //_opTable[0x9c] = new OpCodeRecord(Execute_PUSH);
            //_opTable[0x9d] = new OpCodeRecord(Execute_POP);
            // SAHF - Store SH to flags
            //_opTable[0x9e] = new OpCodeRecord(() => { CondReg.Register = new DataRegister16((byte)(CondReg.Register >> 8), Registers.AH); });
            // LAHF - Load AH from flags
            //_opTable[0x9f] = new OpCodeRecord( () => { Registers.AH = (byte)(CondReg.Register & 0x00ff); });
            //_opTable[0xa0] = new OpCodeRecord(ExecuteMOV_Mem);
            //_opTable[0xa1] = new OpCodeRecord(ExecuteMOV_Mem);
            //_opTable[0xa2] = new OpCodeRecord(ExecuteMOV_Mem);
            //_opTable[0xa3] = new OpCodeRecord(ExecuteMOV_Mem);
            _opTable[0xa4] = new OpCodeRecord(Execute_MoveString);
            _opTable[0xa5] = new OpCodeRecord(Execute_MoveString);
            _opTable[0xa6] = new OpCodeRecord(Execute_CompareString);
            _opTable[0xa7] = new OpCodeRecord(Execute_CompareString);
            //_opTable[0xa8] = new OpCodeRecord(ExecuteLogical_Immediate);
            //_opTable[0xa9] = new OpCodeRecord(ExecuteLogical_Immediate);
            _opTable[0xaa] = new OpCodeRecord(Execute_StoreString);
            _opTable[0xab] = new OpCodeRecord(Execute_StoreString);
            _opTable[0xac] = new OpCodeRecord(Execute_LoadString);
            _opTable[0xad] = new OpCodeRecord(Execute_LoadString);
            _opTable[0xae] = new OpCodeRecord(Execute_ScanString);
            _opTable[0xaf] = new OpCodeRecord(Execute_ScanString);
            //_opTable[0xb0] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb1] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb2] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb3] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb4] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb5] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb6] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb7] = new OpCodeRecord(ExecuteMOV_Imm8);
            //_opTable[0xb8] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xb9] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xba] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xbb] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xbc] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xbd] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xbe] = new OpCodeRecord(ExecuteMOV_Imm16);
            //_opTable[0xbf] = new OpCodeRecord(ExecuteMOV_Imm16);
            // GRP2 rm-8 imm-8 (80186)
            //_opTable[0xc0] = new OpCodeRecord(Execute_RotateAndShift);
            // GRP2 rm-16 imm-16 (80186)
            //_opTable[0xc1] = new OpCodeRecord(Execute_RotateAndShift);

            _opTable[0xc2] = new OpCodeRecord(() => // ret imm-16
            {
                ushort oper = GetImmediate16();
                Bus.IP = Pop();
                Registers.SP += oper;
            });
            _opTable[0xc3] = new OpCodeRecord(() => // ret
            {
                Bus.IP = Pop();
            });
            _opTable[0xc4] = new OpCodeRecord(Execute_LDS_LES);
            _opTable[0xc5] = new OpCodeRecord(Execute_LDS_LES);
            _opTable[0xc6] = new OpCodeRecord(ExecuteMOV_c6);
            _opTable[0xc7] = new OpCodeRecord(ExecuteMOV_c7);
            _opTable[0xc8] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0xc8 is not implemented"); });
            _opTable[0xc9] = new OpCodeRecord(() => { throw new InvalidOperationException("Instruction 0xc9 is not implemented"); });
            _opTable[0xca] = new OpCodeRecord(() => // retf imm-16
            {
                ushort oper = GetImmediate16();
                Bus.IP = Pop();
                Bus.CS = Pop();
                Registers.SP += oper;
            });
            _opTable[0xcb] = new OpCodeRecord(() => // retf
            {
                Bus.IP = Pop();
                Bus.CS = Pop();
            });
            _opTable[0xcc] = new OpCodeRecord(() => { Interrupt(3); });
            _opTable[0xcd] = new OpCodeRecord(() => { Interrupt(Bus.NextIP()); });
            _opTable[0xce] = new OpCodeRecord(() => { if (CondReg.OverflowFlag) Interrupt(4); });
            _opTable[0xcf] = new OpCodeRecord(() => // iret
            {
                Bus.IP = Pop();
                Bus.CS = Pop();
                CondReg.Value = Pop();
            });
            //_opTable[0xd0] = new OpCodeRecord(Execute_RotateAndShift);
            //_opTable[0xd1] = new OpCodeRecord(Execute_RotateAndShift);
            //_opTable[0xd2] = new OpCodeRecord(Execute_RotateAndShift);
            //_opTable[0xd3] = new OpCodeRecord(Execute_RotateAndShift);
            _opTable[0xd4] = new OpCodeRecord(Execute_AsciiAdjustMUL);
            _opTable[0xd5] = new OpCodeRecord(Execute_AsciiAdjustDIV);
            // undocumented SALC instruction
            //_opTable[0xd6] = new OpCodeRecord(() => { if (CondReg.CarryFlag) Registers.AL = 0xff; else Registers.AL = 0x00; });
            //_opTable[0xd7] = new OpCodeRecord(Execute_XLAT);

            // D8-DF ESC OPCODE,SOURCE (to math co-processor)
            // these are unsupported but they use the address byte so this should read
            //_opTable[0xd8] = new OpCodeRecord(() => { Bus.NextIP(); } );
            //_opTable[0xd9] = new OpCodeRecord(() => { Bus.NextIP(); });
            //_opTable[0xda] = new OpCodeRecord(() => { Bus.NextIP(); });
            //_opTable[0xdb] = new OpCodeRecord(() => { Bus.NextIP(); });
            //_opTable[0xdc] = new OpCodeRecord(() => { Bus.NextIP(); });
            //_opTable[0xdd] = new OpCodeRecord(() => { Bus.NextIP(); });
            //_opTable[0xde] = new OpCodeRecord(() => { Bus.NextIP(); });
            //_opTable[0xdf] = new OpCodeRecord(() => { Bus.NextIP(); });

            _opTable[0xe0] = new OpCodeRecord(Execute_Loop);
            _opTable[0xe1] = new OpCodeRecord(Execute_Loop);
            _opTable[0xe2] = new OpCodeRecord(Execute_Loop);
            //_opTable[0xe3] = new OpCodeRecord(Execute_JumpCXZ);
            _opTable[0xe4] = new OpCodeRecord(Execute_IN);
            _opTable[0xe5] = new OpCodeRecord(Execute_IN);
            _opTable[0xe6] = new OpCodeRecord(Execute_OUT);
            _opTable[0xe7] = new OpCodeRecord(Execute_OUT);
            //_opTable[0xe8] = new OpCodeRecord(Execute_CallNear);
            //_opTable[0xe9] = new OpCodeRecord(Execute_JumpNear);
            //_opTable[0xea] = new OpCodeRecord(Execute_JumpFar);
            //_opTable[0xeb] = new OpCodeRecord(Execute_JumpShort);
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
                Bus.IP--;
                Halted = true;
            }); 
            //_opTable[0xf5] = new OpCodeRecord(() => { CondReg.CarryFlag = !CondReg.CarryFlag; });  // CMC - complement carry flag
            _opTable[0xf6] = new OpCodeRecord(Execute_Group3);
            _opTable[0xf7] = new OpCodeRecord(Execute_Group3);
            //_opTable[0xf8] = new OpCodeRecord(() => { CondReg.CarryFlag = false; });  // F8 CLC - clear carry flag
            //_opTable[0xf9] = new OpCodeRecord(() => { CondReg.CarryFlag = true; });  // F9 STC - set carry flag
            //_opTable[0xfa] = new OpCodeRecord(() => { CondReg.InterruptEnable = false; });  // FA CLI - clear interrupt flag
            //_opTable[0xfb] = new OpCodeRecord(() => { CondReg.InterruptEnable = true; });  // FB STI - set interrupt flag
            //_opTable[0xfc] = new OpCodeRecord(() => { CondReg.DirectionFlag = false; });  // FC CLD - clear direction flag
            //_opTable[0xfd] = new OpCodeRecord(() => { CondReg.DirectionFlag = true; });  // FD STD - set direction flag
            _opTable[0xfe] = new OpCodeRecord(Execute_Group4);
            _opTable[0xff] = new OpCodeRecord(Execute_Group5);
        }

        private void Execute_Loop()
        {
            ushort tmp = SignExtend(Bus.NextIP());
            Registers.CX--;
            if( Registers.CX != 0)
            {
                if ((_currentOP == 0xe0 && !CondReg.ZeroFlag) ||  // LOOPNZ
                    (_currentOP == 0xe1 && CondReg.ZeroFlag) ||   // LOOPZ
                    (_currentOP == 0xe2))                       // LOOP
                {
                    Bus.IP += tmp;
                }
            }
        }

        #region Instructions 60-6F (80186)

        private void Execute_Bound()
        {
            // Op 0x62
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

            // for now assume OK

            //int offset = GetSourceData(0, 1, mod, reg, rm);
            //ushort source = _bus.GetData16(offset);

            //offset = GetDestinationData(0, 1, mod, reg, rm);
            //ushort dest = _bus.GetData16(offset);

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

            ushort port;
            if (_currentOP == 0xec || _currentOP == 0xed)
                port = Registers.DX;
            else
                port = Bus.NextIP();

            int word_size = GetWordSize();

            Debug.WriteLine("IN " + port.ToString("X4"));

            if (_inputDevices.TryGetValue(port, out device))
            {
                if (word_size == 0)
                    Registers.AL = device.Read();
                else
                    Registers.AX = device.Read16();
            }
            else
            {
                // if no device is attached, zero out the register
                if (word_size == 0)
                    Registers.AL = 0;
                else
                    Registers.AX = 0;
            }
        }

        private void Execute_IN_String()
        {
            IInputDevice device;
            int word_size = GetWordSize();

            if (_inputDevices.TryGetValue(Registers.DX, out device))
            {
                if (word_size == 0)
                    Bus.SaveByteString(Registers.DI, device.Read());
                else
                    Bus.SaveWordString(Registers.DI, device.Read16());
            }
            else
            {
                if (word_size == 0)
                    Bus.SaveByteString(Registers.DI, 0);
                else
                    Bus.SaveWordString(Registers.DI, 0);
            }
        }   

        private void Execute_OUT_String()
        {
            IOutputDevice device;
            int word_size = GetWordSize();

            if(_outputDevices.TryGetValue(Registers.DX, out device))
            {
                if (word_size == 0)
                    device.Write(Bus.GetByteDestString(Registers.SI));
                else
                    device.Write(Bus.GetWordDestString(Registers.SI));
            }
        }

        // OUT  accumulator is written out to the port
        private void Execute_OUT()
        {
            IOutputDevice device;

            ushort port;
            if (_currentOP == 0xee || _currentOP == 0xef)
                port = Registers.DX;
            else
                port = Bus.NextIP();

            int word_size = GetWordSize();

            Debug.WriteLine("OUT " + port.ToString("X4") + " (data=" + Registers.AL.ToString("X2") + ")");

            if (_outputDevices.TryGetValue(port, out device))
            {
                if (word_size == 0)
                    device.Write(Registers.AL);
                else
                    device.Write(Registers.AX);
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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

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
                source = SignExtend(Bus.NextIP());
            }
            else if (word_size == 0)
            {
                source = Bus.NextIP();

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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

            int word_size = GetWordSize();

            int operand2 = 1;
            if (_currentOP == 0xd2 || _currentOP == 0xd3)
                operand2 = Registers.CL;
            else if (_currentOP == 0xc0 || _currentOP == 0xc1)
            {
                // in this case we need to read the RM data first in the case there
                // is displacement data to read before the immediate data
                int operand1 = GetSourceData(0, word_size, mod, reg, rm);
                operand2 = Bus.NextIP();
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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

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
                            dest = Bus.NextIP();
                        else
                            dest = GetImmediate16();

                        result = source & dest;
                        // Flags: O S Z A P C
                        //        0 x x ? x 0
                        CondReg.OverflowFlag = false;
                        CondReg.CarryFlag = false;
                        CondReg.CalcSignFlag(word_size, result);
                        CondReg.CalcZeroFlag(word_size, result);
                        CondReg.CalcParityFlag(result);
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

                        CondReg.CalcOverflowFlag(word_size, 0, result);
                        CondReg.CalcSignFlag(word_size, result);
                        CondReg.CalcZeroFlag(word_size, result);
                        CondReg.CalcAuxCarryFlag(source, dest);
                        CondReg.CalcParityFlag(result);
                        CondReg.CalcCarryFlag(word_size, result);
                        break;
                    }
                case 0x04: // MUL
                    {
                        if (word_size == 0)
                        {
                            result = source * Registers.AL;
                            Registers.AX = (ushort)result;

                            CondReg.CarryFlag = (Registers.AH != 0);
                            CondReg.OverflowFlag = CondReg.CarryFlag;
                        }
                        else
                        {
                            result = source * Registers.AX;
                            Registers.DX = (ushort)(result >> 16);
                            Registers.AX = (ushort)(result);

                            CondReg.CarryFlag = (Registers.DX != 0);
                            CondReg.OverflowFlag = CondReg.CarryFlag;
                        }
                        break;
                    }
                case 0x05: // IMUL
                    {
                        if (word_size == 0)
                        {
                            Registers.AX = (ushort)(SignExtend((byte)source) * SignExtend(Registers.AL));

                            if ((Registers.AL & 0x80) == 0x80)
                                CondReg.CarryFlag = (Registers.AH != 0xff);
                            else
                                CondReg.CarryFlag = (Registers.AH != 0x00);

                            CondReg.OverflowFlag = CondReg.CarryFlag;
                        }
                        else
                        {
                            result = (int)(SignExtend((byte)source) * SignExtend32(Registers.AX));
                            Registers.DX = (ushort)(result >> 16);
                            Registers.AX = (ushort)(result);

                            if ((Registers.AX & 0x8000) == 0x8000)
                                CondReg.CarryFlag = (Registers.DX != 0xffff);
                            else
                                CondReg.CarryFlag = (Registers.DX != 0x0000);

                            CondReg.OverflowFlag = CondReg.CarryFlag;
                        }
                        break;
                    }
                case 0x06: // DIV
                    {
                        if (word_size == 0)
                        {
                            result = (byte)(Registers.AX / source);
                            Registers.AH = (byte)(Registers.AX % source);
                            Registers.AL = (byte)(result);
                        }
                        else
                        {
                            dest = (Registers.DX << 16) | Registers.AX;
                            Registers.AX = (ushort)(dest / source);
                            Registers.DX = (ushort)(dest % source);
                        }
                        break;
                    }
                case 0x07: // IDIV
                    {
                        if (word_size == 0)
                        {

                            ushort s1 = Registers.AX;
                            ushort s2 = (ushort)source;

                            bool sign = ((s1 ^ s2) & 0x8000) == 0x8000;

                            if (s1 >= 0x8000)
                                s1 = (ushort)(~s1 + 1);
                            if (s2 >= 0x8000)
                                s2 = (ushort)(~s2 + 1);

                            ushort d1 = (ushort)(s1 / s2);
                            ushort d2 = (ushort)(s1 % s2);

                            if (sign)
                            {
                                d1 = (ushort)(~d1 + 1);
                                d2 = (ushort)(~d2 + 1);
                            }

                            Registers.AL = (byte)d1;
                            Registers.AH = (byte)d2;
                        }
                        else
                        {

                            uint dxax = (uint)((Registers.DX << 16) | Registers.AX);
                            uint divisor = SignExtend32((ushort)source);

                            bool sign = ((dxax ^ divisor) & 0x80000000) == 0x80000000;

                            if (dxax >= 0x80000000)
                                dxax = (uint)(~dxax + 1);

                            if (divisor >= 0x80000000)
                                divisor = (uint)(~divisor + 1);

                            uint d1 = (uint)(dxax / divisor);
                            uint d2 = (uint)(dxax % divisor);

                            if (sign)
                            {
                                d1 = (uint)((~d1 + 1) & 0xffff);
                                d2 = (uint)((~d2 + 1) & 0xffff);
                            }

                            Registers.AX = (ushort)d1;
                            Registers.DX = (ushort)d2;
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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

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
                        Push(Bus.IP);
                        Bus.IP = (ushort) oper;
                        break;
                    }
                case 0x03:
                    {
                        // CALL mem-16 (intersegment)
                        Push(Bus.CS);
                        Push(Bus.IP);
                        Bus.IP = Bus.GetWord(oper);
                        Bus.CS = Bus.GetWord(oper + 2);
                        break;
                    }
                case 0x04:
                    {
                        // JMP reg/mem=16 (intrasegment)
                        Bus.IP = (ushort) oper;
                        break;
                    }
                case 0x05:
                    {
                        // JMP mem-16 (intersegment)
                        Bus.IP = Bus.GetWord(oper);
                        Bus.CS = Bus.GetWord(oper + 2);
                        break;
                    }
                case 0x06:
                    {
                        // push mem-16
                        Push((ushort)oper);
                        break;
                    }
            }
        }

        #endregion

        #region Jump/Call Instructions

        private void Execute_CallNear()
        {
            ushort oper = GetImmediate16();
            Push(Bus.IP);
            Bus.IP += oper;
        }

        private void Execute_CallFar()
        {
            ushort nextIP = GetImmediate16();
            ushort nextCS = GetImmediate16();
            Push(Bus.CS);
            Push(Bus.IP);
            Bus.IP = nextIP;
            Bus.CS = nextCS;
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
                        jumping = CondReg.OverflowFlag;
                        break;
                    }
                case 0x71:
                    {
                        jumping = !CondReg.OverflowFlag;
                        break;
                    }
                case 0x72:
                    {
                        jumping = CondReg.CarryFlag;
                        break;
                    }
                case 0x73:
                    {
                        jumping = !CondReg.CarryFlag;
                        break;
                    }
                case 0x74:
                    {
                        jumping = CondReg.ZeroFlag;
                        break;
                    }
                case 0x75:
                    {
                        jumping = !CondReg.ZeroFlag;
                        break;
                    }
                case 0x76:
                    {
                        jumping = (CondReg.CarryFlag | CondReg.ZeroFlag);
                        break;
                    }
                case 0x77:
                    {
                        jumping = !(CondReg.CarryFlag | CondReg.ZeroFlag);
                        break;
                    }
                case 0x78:
                    {
                        jumping = CondReg.SignFlag;
                        break;
                    }
                case 0x79:
                    {
                        jumping = !CondReg.SignFlag;
                        break;
                    }
                case 0x7a:
                    {
                        jumping = CondReg.ParityFlag;
                        break;
                    }
                case 0x7b:
                    {
                        jumping = !CondReg.ParityFlag;
                        break;
                    }
                case 0x7c:
                    {
                        jumping = (CondReg.SignFlag ^ CondReg.OverflowFlag);
                        break;
                    }
                case 0x7d:
                    {
                        jumping = !(CondReg.SignFlag ^ CondReg.OverflowFlag);
                        break;
                    }
                case 0x7e:
                    {
                        jumping = ((CondReg.SignFlag ^ CondReg.OverflowFlag) | CondReg.ZeroFlag);
                        break;
                    }
                case 0x7f:
                    {
                        jumping = !((CondReg.SignFlag ^ CondReg.OverflowFlag) | CondReg.ZeroFlag);
                        break;
                    }
            }
            if (jumping)
                Execute_JumpShort();
            else
                Bus.NextIP();  
        }
        private void Execute_JumpCXZ()
        {
            if (Registers.CX == 0)
                Execute_JumpShort();
            else
                Bus.NextIP();
        }
        private void Execute_JumpShort()
        {
            // add to IP "after" the instruction is complete
            ushort oper = SignExtend(Bus.NextIP());
            Bus.IP += oper;
        }
        private void Execute_JumpNear()
        {
            ushort oper = GetImmediate16();
            Bus.IP += oper;
        }
        private void Execute_JumpFar()
        {
            ushort nextIP = GetImmediate16();
            ushort nextCS = GetImmediate16();
            Bus.IP = nextIP;
            Bus.CS = nextCS;
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
                    Bus.MoveByteString(Registers.SI, Registers.DI);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.SI--;
                        Registers.DI--;
                    }
                    else
                    {
                        Registers.SI++;
                        Registers.DI++;
                    }
                }
                else
                {
                    Bus.MoveWordString(Registers.SI, Registers.DI);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.SI -= 2;
                        Registers.DI -= 2;
                    }
                    else
                    {
                        Registers.SI += 2;
                        Registers.DI += 2;
                    }
                }
                if (_repeat) Registers.CX--;

                if (_repeatType == 1 && !CondReg.ZeroFlag)
                    _repeat = false;
                if (_repeatType == 2 && CondReg.ZeroFlag)
                    _repeat = false;

            } while (_repeat && Registers.CX != 0);
            _repeat = false;
        }
        private void Execute_CompareString()
        {
            int word_size = GetWordSize();
            int result = 0;
            int source = 0;
            int dest = Bus.GetData(word_size, Registers.SI);

            // NOTE: the concept of "source" and "dest" for subtraction here is flipped.

            do
            {
                if (word_size == 0)
                {
                    source = Bus.GetByteDestString(Registers.DI);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.SI--;
                        Registers.DI--;
                    }
                    else
                    {
                        Registers.SI++;
                        Registers.DI++;
                    }
                }
                else
                {
                    source = Bus.GetWordDestString(Registers.DI);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.SI -= 2;
                        Registers.DI -= 2;
                    }
                    else
                    {
                        Registers.SI += 2;
                        Registers.DI += 2;
                    }
                }

                result = dest - source;
                CondReg.CalcOverflowFlag(word_size, source, dest);
                CondReg.CalcSignFlag(word_size, result);
                CondReg.CalcZeroFlag(word_size, result);
                CondReg.CalcAuxCarryFlag(source, dest);
                CondReg.CalcParityFlag(result);
                CondReg.CalcCarryFlag(word_size, result);
                if (_repeat) Registers.CX--;

                if (_repeatType == 1 && !CondReg.ZeroFlag)
                    _repeat = false;
                if (_repeatType == 2 && CondReg.ZeroFlag)
                    _repeat = false;

            } while (_repeat && Registers.CX != 0);
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
                    dest = Bus.GetByteDestString(Registers.DI);
                    source = Registers.AL;
                    if (CondReg.DirectionFlag)
                    {
                        Registers.DI--;
                    }
                    else
                    {
                        Registers.DI++;
                    }
                }
                else
                {
                    dest = Bus.GetWordDestString(Registers.DI);
                    source = Registers.AX;
                    if (CondReg.DirectionFlag)
                    {
                        Registers.DI -= 2;
                    }
                    else
                    {
                        Registers.DI += 2;
                    }
                }

                result = dest - source;
                CondReg.CalcOverflowFlag(word_size, source, dest);
                CondReg.CalcSignFlag(word_size, result);
                CondReg.CalcZeroFlag(word_size, result);
                CondReg.CalcAuxCarryFlag(source, dest);
                CondReg.CalcParityFlag(result);
                CondReg.CalcCarryFlag(word_size, result);

                if (_repeat) Registers.CX--;

                if (_repeatType == 1 && !CondReg.ZeroFlag)
                    _repeat = false;
                if (_repeatType == 2 && CondReg.ZeroFlag)
                    _repeat = false;

            } while (_repeat && Registers.CX != 0);
            _repeat = false;
        }
        private void Execute_StoreString()
        {
            int word_size = GetWordSize();
            do
            {
                if (word_size == 0)
                {
                    Bus.SaveByteString(Registers.DI, Registers.AL);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.DI--;
                    }
                    else
                    {
                        Registers.DI++;
                    }
                }
                else
                {
                    Bus.SaveWordString(Registers.DI, Registers.AX);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.DI -= 2;
                    }
                    else
                    {
                        Registers.DI += 2;
                    }
                }
                if (_repeat) Registers.CX--;
            } while (_repeat && Registers.CX != 0);
            _repeat = false;
        }
        private void Execute_LoadString()
        {
            int word_size = GetWordSize();
            do
            {
                if (word_size == 0)
                {
                    Registers.AL = Bus.GetByte(Registers.SI);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.SI--;
                    }
                    else
                    {
                        Registers.SI++;
                    }
                }
                else
                {
                    Registers.AX = Bus.GetWord(Registers.SI);
                    if (CondReg.DirectionFlag)
                    {
                        Registers.SI -= 2;
                    }
                    else
                    {
                        Registers.SI += 2;
                    }
                }
                if (_repeat) Registers.CX--;
            } while (_repeat && Registers.CX != 0);
            _repeat = false;
        }
        #endregion

        #region BCD adjustment instructions

        private void Execute_AsciiAdjustMUL()
        {
            Registers.AH = (byte)(Registers.AL / 10);
            Registers.AL = (byte)(Registers.AL % 10);
            CondReg.CalcParityFlag(Registers.AX);
            CondReg.CalcSignFlag(1, Registers.AX);
            CondReg.CalcZeroFlag(1, Registers.AX);
        }
        private void Execute_AsciiAdjustDIV()
        {
            Registers.AL += (byte)(Registers.AH * 10);
            Registers.AH = 0;
            CondReg.CalcParityFlag(Registers.AX);
            CondReg.CalcSignFlag(1, Registers.AX);
            CondReg.CalcZeroFlag(1, Registers.AX);
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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

            int offset = 0;

            AssertMOD(mod);
            switch (mod)
            {
                case 0x00:
                    {
                        offset = Bus.GetWord(GetRMTable1(rm));
                        break;
                    }
                case 0x01:
                case 0x02:   // difference is processed in the GetRMTable2 function
                    {
                        offset = Bus.GetWord(GetRMTable2(mod, rm));
                        break;
                    }
                case 0x03:
                    {
                        throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", _currentOP));
                    }
            }

            SaveRegField16(reg, Bus.GetWord(offset));

            if (_currentOP == 0xc4)
                Bus.ES = Bus.GetWord(offset + 2);
            else
                Bus.DS = Bus.GetWord(offset + 2);
        }

        #endregion

        #region MOV instructions

        private void ExecuteMOV_c6()
        {
            // MOV MEM-8, IMM-8
            // displacement bytes are optional so don't retrieve the immediate value
            // until the destination offset has been determined.
            byte mod = 0, reg = 0, rm = 0;
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);
            
            int dest;
            AssertMOD(mod);
            switch (mod)
            {
                case 0x00:
                    {
                        dest = GetRMTable1(rm);
                        Bus.SaveByte(dest, Bus.NextIP());
                        break;
                    }
                case 0x01:
                    {
                        dest = GetRMTable2(mod, rm);
                        Bus.SaveByte(dest, Bus.NextIP());
                        break;
                    }
                case 0x02:
                    {
                        dest = GetRMTable2(mod, rm);
                        Bus.SaveByte(dest, Bus.NextIP());
                        break;
                    }
                case 0x03:
                    {
                        SaveRegField8(rm, Bus.NextIP());
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
            SplitAddrByte(Bus.NextIP(), ref mod, ref reg, ref rm);

            int dest;
            AssertMOD(mod);
            switch (mod)
            {
                case 0x00:
                    {
                        dest = GetRMTable1(rm);
                        Bus.SaveWord(dest, GetImmediate16());
                        break;
                    }
                case 0x01:
                    {
                        dest = GetRMTable2(mod, rm);
                        Bus.SaveWord(dest, GetImmediate16());
                        break;
                    }
                case 0x02:
                    {
                        dest = GetRMTable2(mod, rm);
                        Bus.SaveWord(dest, GetImmediate16());
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
                    SaveSegRegField(reg, (ushort)data);
                }
                else
                {
                    if (word_size == 0)
                    {
                        SaveRegField8(reg, (byte)data);
                    }
                    else
                    {
                        SaveRegField16(reg, (ushort)data);
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
                                SaveRegField8(rm, (byte)data);
                            }
                            else // if ((direction == 0) && (word_size == 1))
                            {
                                SaveRegField16(rm, (ushort)data);
                            }
                            break;
                        }
                }
            }
        }
        #endregion

        #region Interrupt Processing
        public void Interrupt(int interrupt_num)
        {
            // address of pointer is calculated by multiplying interrupt type by 4
            ushort int_ptr = (ushort)(interrupt_num * 4);

            // push flags
            Push(CondReg.Value);

            // push CS and IP
            Push(Bus.CS);
            Push(Bus.IP);
            
            // clear trap flag
            CondReg.TrapFlag = false;

            // clear interrupt enable
            CondReg.InterruptEnable = false;

            // the second word of the interrupt pointer replaces CS
            Bus.CS = Bus.GetWord(0, int_ptr + 2);

            // replace IP by first word of interrupt pointer
            Bus.IP = Bus.GetWord(0, int_ptr);
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
            CondReg.CalcOverflowFlag(1, source, dest);
            CondReg.CalcSignFlag(1, result);
            CondReg.CalcZeroFlag(1, result);
            CondReg.CalcAuxCarryFlag(source, dest);
            CondReg.CalcParityFlag(result);
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
            CondReg.CalcOverflowSubtract(1, source, dest);
            CondReg.CalcSignFlag(1, result);
            CondReg.CalcZeroFlag(1, result);
            CondReg.CalcAuxCarryFlag(source, dest);
            CondReg.CalcParityFlag(result);
        }

        private void ADD_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool with_carry)
        {
            AssertMOD(mod);
            int result = 0;
            int offset;
            int dest = 0;
            int carry = 0;

            // Include carry flag if necessary
            if (with_carry && CondReg.CarryFlag)
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
                    SaveRegField16(reg, (ushort)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(word_size, offset);
                            result = source + dest + carry;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = source + dest + carry;
                            Bus.SaveData(word_size, offset, result);
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
                                SaveRegField16(rm, (ushort)result);
                            }
                            break;
                        }
                }
            }


            // Flags: O S Z A P C
            CondReg.CalcOverflowFlag(word_size, source, dest);
            CondReg.CalcSignFlag(word_size, result);
            CondReg.CalcZeroFlag(word_size, result);
            CondReg.CalcAuxCarryFlag(source, dest);
            CondReg.CalcParityFlag(result);
            CondReg.CalcCarryFlag(word_size, result);
        }

        private void SUB_Destination(int source, int direction, int word_size, byte mod, byte reg, byte rm, bool with_carry, bool comp_only)
        {
            AssertMOD(mod);
            int result = 0;
            int offset;
            int dest = 0;
            int carry = 0;

            // Include carry flag if necessary
            if (with_carry && CondReg.CarryFlag)
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
                    if (!comp_only) SaveRegField16(reg, (ushort)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest - (source + carry);
                            if (!comp_only) Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest - (source + carry);
                            if (!comp_only) Bus.SaveData(word_size, offset, result);
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
                                if (!comp_only) SaveRegField16(rm, (ushort)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            CondReg.CalcOverflowSubtract(word_size, source + carry, dest);
            CondReg.CalcSignFlag(word_size, result);
            CondReg.CalcZeroFlag(word_size, result);
            CondReg.CalcAuxCarryFlag(source, dest);
            CondReg.CalcParityFlag(result);
            CondReg.CalcCarryFlag(word_size, result);
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
                    if (!test_only) SaveRegField16(reg, (ushort)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest & source;
                            if (!test_only) Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest & source;
                            if (!test_only) Bus.SaveData(word_size, offset, result);
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
                                if (!test_only) SaveRegField16(rm, (ushort)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            CondReg.OverflowFlag = false;
            CondReg.CarryFlag = false;
            CondReg.CalcSignFlag(word_size, result);
            CondReg.CalcZeroFlag(word_size, result);
            CondReg.CalcParityFlag(result);
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
                    SaveRegField16(reg, (ushort)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest | source;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest | source;
                            Bus.SaveData(word_size, offset, result);
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
                                SaveRegField16(rm, (ushort)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            CondReg.OverflowFlag = false;
            CondReg.CarryFlag = false;
            CondReg.CalcSignFlag(word_size, result);
            CondReg.CalcZeroFlag(word_size, result);
            CondReg.CalcParityFlag(result);
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
                    SaveRegField16(reg, (ushort)result);
                }
            }
            else
            {
                switch (mod)
                {
                    case 0x00:
                        {
                            offset = GetRMTable1(rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest ^ source;
                            Bus.SaveData(word_size, offset, result);
                            break;
                        }
                    case 0x01:
                    case 0x02:  // difference is processed in the GetRMTable2 function
                        {
                            offset = GetRMTable2(mod, rm);
                            dest = Bus.GetData(word_size, offset);
                            result = dest ^ source;
                            Bus.SaveData(word_size, offset, result);
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
                                SaveRegField16(rm, (ushort)result);
                            }
                            break;
                        }
                }
            }

            // Flags: O S Z A P C
            //        0 x x ? x 0
            CondReg.OverflowFlag = false;
            CondReg.CarryFlag = false;
            CondReg.CalcSignFlag(word_size, result);
            CondReg.CalcZeroFlag(word_size, result);
            CondReg.CalcParityFlag(result);
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
                        original = Bus.GetByte(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetByte(offset);
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
                old_CF = CondReg.CarryFlag;

                // carry bit equal to high bit
                CondReg.CarryFlag = ((result & 0x80) == 0x80);

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
                        if (CondReg.CarryFlag)
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
                    CondReg.OverflowFlag = ((original & 0xc0) != (result & 0xc0));
        }
                else
                {
                    // when rotating 1, if the sign changes as a result of the rotate, set OF
                    CondReg.OverflowFlag = ((original ^ result) & 0x80) == 0x80;
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
                        original = Bus.GetWord(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetWord(offset);
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
                old_CF = CondReg.CarryFlag;

                // carry bit equal to high bit
                CondReg.CarryFlag = ((result & 0x8000) == 0x8000);

                // shift left
                result = (ushort)(result << 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes low bit
                    // otherwise the original high bit (which is now the CF) becomes low bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (ushort)(result | 0x0001);
                    }
                    else
                    {
                        if (CondReg.CarryFlag)
                            result = (ushort)(result | 0x0001);
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
                    CondReg.OverflowFlag = ((original & 0xc000) != (result & 0xc000));
                }
                else
                {
                    // when rotating 1, if the sign changes as a result of the rotate, set OF
                    CondReg.OverflowFlag = ((original ^ result) & 0x8000) == 0x8000;
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
                        original = Bus.GetByte(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetByte(offset) ;
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
                old_CF = CondReg.CarryFlag;

                // carry bit equal to low bit
                CondReg.CarryFlag = ((result & 0x01) == 0x01);

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
                        if (CondReg.CarryFlag)
                            result = (byte)(result | 0x80);
                    }
                }

                // if arithmetic shift then the sign bit retains its original value
                // set SF, ZF, PF
                if( arithmetic_shift )
                {
                    result = (byte)(result | (byte)(original & 0x80));
                    CondReg.CalcSignFlag(0, result);
                    CondReg.CalcZeroFlag(0, result);
                    CondReg.CalcParityFlag(result);
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
                    CondReg.OverflowFlag = false;
                }
                // if shift, set OF if the sign has changed
                else if (shift_only)
                {
                    CondReg.OverflowFlag = ((original ^ result) & 0x80) == 0x80;
                }
                // if rotate through carry, set OF if high order bit and carry flags have changed
                else if( !shift_only & through_carry )
                {
                    CondReg.OverflowFlag = (((original ^ result) & 0x80) == 0x80) & (old_CF == CondReg.CarryFlag);
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
                        original = Bus.GetWord(offset);
                        break;
                    }
                case 0x01:
                case 0x02:  // difference is processed in the GetRMTable2 function
                    {
                        offset = GetRMTable2(mod, rm);
                        original = Bus.GetWord(offset);
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
                old_CF = CondReg.CarryFlag;

                // carry bit equal to low bit
                CondReg.CarryFlag = ((result & 0x0001) == 0x0001);

                // shift right
                result = (ushort)(result >> 1);

                if (!shift_only)
                {
                    // if through carry, the original CF value becomes high bit
                    // otherwise the original low bit (which is now the CF) becomes high bit
                    if (through_carry)
                    {
                        if (old_CF)
                            result = (ushort)(result | 0x8000);
                    }
                    else
                    {
                        if (CondReg.CarryFlag)
                            result = (ushort)(result | 0x8000);
                    }
                }

                // if arithmetic shift then the sign bit retains its original value
                if (arithmetic_shift)
                {
                    result = (byte)(result | (byte)(original & 0x80));
                    CondReg.CalcSignFlag(1, result);
                    CondReg.CalcZeroFlag(1, result);
                    CondReg.CalcParityFlag(result);
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
                    CondReg.OverflowFlag = false;
                }
                // if shift, set OF if the sign has changed
                else if (shift_only)
                {
                    CondReg.OverflowFlag = ((original ^ result) & 0x8000) == 0x8000;
                }
                // if rotate through carry, set OF if high order bit and carry flags have changed
                else if (!shift_only & through_carry)
                {
                    CondReg.OverflowFlag = (((original ^ result) & 0x8000) == 0x8000) & (old_CF == CondReg.CarryFlag);
                }
            }
        }

        private void Push(ushort value)
        {
            Registers.SP -= 2;
            Bus.PushStack(Registers.SP, value);
        }

        private ushort Pop()
        {
            ushort result = Bus.PopStack(Registers.SP);
            Registers.SP += 2;
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
                        result = Registers.AL;
                        break;
                    }
                case 0x01:
                    {
                        result = Registers.CL;
                        break;
                    }
                case 0x02:
                    {
                        result = Registers.DL;
                        break;
                    }
                case 0x03:
                    {
                        result = Registers.BL;
                        break;
                    }
                case 0x04:
                    {
                        result = Registers.AH;
                        break;
                    }
                case 0x05:
                    {
                        result = Registers.CH;
                        break;
                    }
                case 0x06:
                    {
                        result = Registers.DH;
                        break;
                    }
                case 0x07:
                    {
                        result = Registers.BH;
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
                       Registers.AL = value;
                        break;
                    }
                case 0x01:
                    {
                        Registers.CL = value;
                        break;
                    }
                case 0x02:
                    {
                        Registers.DL = value;
                        break;
                    }
                case 0x03:
                    {
                        Registers.BL = value;
                        break;
                    }
                case 0x04:
                    {
                        Registers.AH = value;
                        break;
                    }
                case 0x05:
                    {
                        Registers.CH = value;
                        break;
                    }
                case 0x06:
                    {
                        Registers.DH = value;
                        break;
                    }
                case 0x07:
                    {
                        Registers.BH = value;
                        break;
                    }
            }

        }

        // Get 16 bit REG result (or R/M mod=11)
        private ushort GetRegField16(byte reg)
        {
            ushort result = 0;
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        result = Registers.AX;
                        break;
                    }
                case 0x01:
                    {
                        result = Registers.CX;
                        break;
                    }
                case 0x02:
                    {
                        result = Registers.DX;
                        break;
                    }
                case 0x03:
                    {
                        result = Registers.BX;
                        break;
                    }
                case 0x04:
                    {
                        result = Registers.SP;
                        break;
                    }
                case 0x05:
                    {
                        result = Registers.BP;
                        break;
                    }
                case 0x06:
                    {
                        result = Registers.SI;
                        break;
                    }
                case 0x07:
                    {
                        result = Registers.DI;
                        break;
                    }
            }
            return result;
        }

        // Save 16 bit value in register indicated by REG
        private void SaveRegField16(byte reg, ushort value)
        {
            AssertREG(reg);
            switch (reg)
            {
                case 0x00:
                    {
                        Registers.AX = value;
                        break;
                    }
                case 0x01:
                    {
                        Registers.CX = value;
                        break;
                    }
                case 0x02:
                    {
                        Registers.DX = value;
                        break;
                    }
                case 0x03:
                    {
                        Registers.BX = value;
                        break;
                    }
                case 0x04:
                    {
                        Registers.SP = value;
                        break;
                    }
                case 0x05:
                    {
                        Registers.BP = value;
                        break;
                    }
                case 0x06:
                    {
                        Registers.SI = value;
                        break;
                    }
                case 0x07:
                    {
                        Registers.DI = value;
                        break;
                    }
            }

        }

        // Get 16 bit SREG result
        private ushort GetSegRegField(byte reg)
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
        private void SaveSegRegField(byte reg, ushort value)
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

        #region R/M Tables
        // R/M Table 1 (mod=00)
        // returns the offset as a result of the operand
        private int GetRMTable1(byte rm)
        {
            ushort result = 0;
            AssertRM(rm);
            switch (rm)
            {
                case 0x00:
                    {
                        result = (ushort)(Registers.BX + Registers.SI);
                        break;
                    }
                case 0x01:
                    {
                        result = (ushort)(Registers.BX + Registers.DI);
                        break;
                    }
                case 0x02:
                    {
                        result = (ushort)(Registers.BP + Registers.SI);
                        break;
                    }
                case 0x03:
                    {
                        result = (ushort)(Registers.BP + Registers.DI);
                        break;
                    }
                case 0x04:
                    {
                        result = Registers.SI;
                        break;
                    }
                case 0x05:
                    {
                        result = Registers.DI;
                        break;
                    }
                case 0x06:
                    {
                        // direct address
                        // There is only one RM table lookup per instruction but this may be invoked more than once
                        // for certain instructions.  For this reason we preserve the offset in case it is needed again.
                        if (_RMTableLookupCount == InstructionCount)
                        {
                            result = _RMTableLastLookup;
                        }
                        else
                        {
                            result = GetImmediate16();
                            _RMTableLastLookup = result;
                            _RMTableLookupCount = InstructionCount;
                            
                        }
                        break;
                    }
                case 0x07:
                    {
                        result = Registers.BX;
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
            ushort result = 0;
            ushort disp = 0;
            AssertRM(rm);
            switch (rm)
            {
                case 0x00:
                    {
                        result = (ushort)(Registers.BX + Registers.SI);
                        break;
                    }
                case 0x01:
                    {
                        result = (ushort)(Registers.BX + Registers.DI);
                        break;
                    }
                case 0x02:
                    {
                        result = (ushort)(Registers.BP + Registers.SI);
                        break;
                    }
                case 0x03:
                    {
                        result = (ushort)(Registers.BP + Registers.DI);
                        break;
                    }
                case 0x04:
                    {
                        result = Registers.SI;
                        break;
                    }
                case 0x05:
                    {
                        result = Registers.DI;
                        break;
                    }
                case 0x06:
                    {
                        Bus.UsingBasePointer = true;
                        result = Registers.BP;
                        break;
                    }
                case 0x07:
                    {
                        result = Registers.BX;
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
                        if (_RMTableLookupCount == InstructionCount)
                        {
                            disp = _RMTableLastLookup;
                        }
                        else
                        {
                            disp = Bus.NextIP();
                            _RMTableLastLookup = disp;
                            _RMTableLookupCount = InstructionCount;
                        }
                        break;
                    }
                case 0x02:
                    {
                        // 16 bit displacement
                        if (_RMTableLookupCount == InstructionCount)
                        {
                            disp = _RMTableLastLookup;
                        }
                        else
                        {
                            disp = GetImmediate16();
                            _RMTableLastLookup = disp;
                            _RMTableLookupCount = InstructionCount;
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("mod", mod, string.Format("Invalid mod value in opcode={0:X2}", _currentOP));
                    }
            }
            return (ushort)(result + disp);

        }
        #endregion

        #region Helper and Utility functions

        // Gets the immediate 16 bit value
        private ushort GetImmediate16()
        {
            byte lo = Bus.NextIP();
            byte hi = Bus.NextIP();
            return new WordRegister(hi, lo);
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
        private ushort SignExtend(byte num)
        {
            if (num < 0x80)
                return num;
            else
                return new WordRegister(0xff, num);
        }

        // Sign extend 16 bits to 32 bits
        private uint SignExtend32(ushort num)
        {
            if (num < 0x8000)
                return num;
            else
                return num | 0xffffff00;
        }

        #endregion  
    }
}
