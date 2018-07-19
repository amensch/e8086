using System;
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

        // Preserve the current OP code
        private byte CurrentOpCode;

        // Table of OpCodes
        private Dictionary<int, Instruction> instructions = new Dictionary<int, Instruction>();

        // General Registers: AX, BX, CX, DX and SP, BP, SI, DI
        public Registers Registers { get; private set; } = new Registers();

        // Flags
        public ConditionalRegister CondReg { get; private set; } = new ConditionalRegister();

        // Bus Interface Unit
        public IBus Bus { get; private set; }

        // I/O Ports
        private Dictionary<int, IInputDevice> InputDevices;
        private Dictionary<int, IOutputDevice> OutputDevices;

        // Repeat flag
        public RepeatModeEnum RepeatMode { get; set; }

        // Property to indicate a halt has been encountered
        public bool Halted { get; set; }

        public ExecutionUnit(IBus bus)
        {
            Bus = bus;
            Halted = false;
            RepeatMode = RepeatModeEnum.NoRepeat;

            InputDevices = new Dictionary<int, IInputDevice>();
            OutputDevices = new Dictionary<int, IOutputDevice>();

            LoadInstructionList();
        }

        public void Tick()
        {

            // If the trap flag is set, trigger interrupt 1
            if (CondReg.TrapFlag)
            {
                var ins = new INT(0, 1, this, Bus);
                ins.Execute();
            }

            // If interrupts are enabled and there is an interrupt waiting,
            // trigger the next interrupt.  Only one interrupt is processed per
            // tick if multiple are waiting.
            if( CondReg.InterruptEnable ) // && i8259 has an interrupt waiting
            {
                // Interrupt( i8259 next interrupt ) ;
            }

            // Reset the prefix and base pointer flags.
            Bus.SegmentOverride = SegmentOverrideState.NoOverride;
            Bus.UsingBasePointer = false;
            RepeatMode = RepeatModeEnum.NoRepeat;

            // Process prefix instructions
            bool more = false;
            do
            {
                // Retrieve the next instruction and count stats
                InstructionCount++;
                CurrentOpCode = Bus.NextIP();
                Stats.AddOpCode(CurrentOpCode);
                more = true;
                switch (CurrentOpCode)
                {
                    case 0x26:
                        {
                            Bus.SegmentOverride = SegmentOverrideState.UseES;
                            break;
                        }
                    case 0x2e:
                        {
                            Bus.SegmentOverride = SegmentOverrideState.UseCS;
                            break;
                        }
                    case 0x36:
                        {
                            Bus.SegmentOverride = SegmentOverrideState.UseSS;
                            break;
                        }
                    case 0x3e:
                        {
                            Bus.SegmentOverride = SegmentOverrideState.UseDS;
                            break;
                        }
                    case 0xf2:
                        {
                            RepeatMode = RepeatModeEnum.REPNZ;
                            break;
                        }
                    case 0xf3:
                        {
                            RepeatMode = RepeatModeEnum.REP;
                            break;
                        }
                    default:
                        {
                            more = false;
                            break;
                        }
                }
            } while (more);

            // Process the next instruction. 
            if (instructions.ContainsKey(CurrentOpCode))
            {
                instructions[CurrentOpCode].Execute();
            }
            else
            {
                var ins = new InvalidInstruction(CurrentOpCode, this, Bus);
                ins.Execute();
            }

            // NOTE: a current minor flaw here is if there is a repeat instruction because the entire loop
            // will get executed immediately without allowing for any interrupts.  Watch for timing issues
            // with PIT if there is a long loop.

            // Tick the PIT
            // i8253.Tick()


        }

        public void AddInputDevice(int port, IInputDevice device)
        {
            if (InputDevices.ContainsKey(port))
            {
                InputDevices.Remove(port);
            }
            InputDevices.Add(port, device);
        }

        public void AddOutputDevice(int port, IOutputDevice device)
        {
            if (OutputDevices.ContainsKey(port))
            {
                OutputDevices.Remove(port);
            }
            OutputDevices.Add(port, device);
        }

        private void LoadInstructionList()
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
            instructions.Add(0x6c, new INS(0x6c, this, Bus));
            instructions.Add(0x6d, new INS(0x6d, this, Bus));
            instructions.Add(0x6e, new OUTS(0x6e, this, Bus));
            instructions.Add(0x6f, new OUTS(0x6f, this, Bus));
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
            instructions.Add(0xa4, new MOVS(0xa4, this, Bus));
            instructions.Add(0xa5, new MOVS(0xa5, this, Bus));
            instructions.Add(0xa6, new CMPS(0xa6, this, Bus));
            instructions.Add(0xa7, new CMPS(0xa7, this, Bus));
            instructions.Add(0xa8, new TEST_Immediate(0xa8, this, Bus));
            instructions.Add(0xa9, new TEST_Immediate(0xa9, this, Bus));
            instructions.Add(0xaa, new STOS(0xaa, this, Bus));
            instructions.Add(0xab, new STOS(0xab, this, Bus));
            instructions.Add(0xac, new LODS(0xac, this, Bus));
            instructions.Add(0xad, new LODS(0xad, this, Bus));
            instructions.Add(0xae, new SCAS(0xae, this, Bus));
            instructions.Add(0xaf, new SCAS(0xaf, this, Bus));

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
            instructions.Add(0xc2, new RET_ImmediateWord(0xc2, this, Bus));
            instructions.Add(0xc3, new RET(0xc3, this, Bus));
            instructions.Add(0xc4, new LES(0xc4, this, Bus));
            instructions.Add(0xc5, new LDS(0xc5, this, Bus));
            instructions.Add(0xc6, new MOV_ByteImmediateToMemory(0xc6, this, Bus));
            instructions.Add(0xc7, new MOV_WordImmediateToMemory(0xc7, this, Bus));
            instructions.Add(0xc8, new NOOP(0xc8, this, Bus));  // undocumented and unimplemented
            instructions.Add(0xc9, new NOOP(0xc9, this, Bus));  // undocumented and unimplemented

            instructions.Add(0xca, new RETF_ImmediateWord(0xca, this, Bus));
            instructions.Add(0xcb, new RETF(0xcb, this, Bus));
            instructions.Add(0xcc, new INT(0xcc, 3, this, Bus));
            instructions.Add(0xcd, new INT(0xcd, this, Bus));
            instructions.Add(0xce, new INT(0xce, CondReg.OverflowFlag, 4, this, Bus));

            instructions.Add(0xcf, new IRET(0xcf, this, Bus));
            instructions.Add(0xd0, new RotateAndShift(0xd0, this, Bus));
            instructions.Add(0xd1, new RotateAndShift(0xd1, this, Bus));
            instructions.Add(0xd2, new RotateAndShiftCL(0xd2, this, Bus));
            instructions.Add(0xd3, new RotateAndShiftCL(0xd3, this, Bus));
            instructions.Add(0xd4, new AAM(0xd4, this, Bus));
            instructions.Add(0xd5, new AAD(0xd5, this, Bus));

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
            instructions.Add(0xe0, new LOOPNE(0xe0, this, Bus));
            instructions.Add(0xe1, new LOOPE(0xe1, this, Bus));
            instructions.Add(0xe2, new LOOP(0xe2, this, Bus));
            instructions.Add(0xe3, new JCXZ(0xe3, this, Bus));
            instructions.Add(0xe4, new IN_Reg(0xe4, this, Bus));
            instructions.Add(0xe5, new IN_Reg(0xe5, this, Bus));
            instructions.Add(0xe6, new OUT_Reg(0xe6, this, Bus));
            instructions.Add(0xe7, new OUT_Reg(0xe7, this, Bus));
            instructions.Add(0xe8, new CALL_Near(0xe8, this, Bus));
            instructions.Add(0xe9, new JMP_Near(0xe9, this, Bus));
            instructions.Add(0xea, new JMP_Far(0xea, this, Bus));
            instructions.Add(0xeb, new JMP(0xeb, this, Bus));
            instructions.Add(0xec, new IN_Immediate(0xec, this, Bus));
            instructions.Add(0xed, new IN_Immediate(0xed, this, Bus));
            instructions.Add(0xee, new OUT_Immediate(0xee, this, Bus));
            instructions.Add(0xef, new OUT_Immediate(0xef, this, Bus));
            instructions.Add(0xf0, new NOOP(0xf0, this, Bus));  // LOCK instruction
            instructions.Add(0xf1, new NOOP(0xf1, this, Bus));  // officially undocumented opcode
            // 0xf2: REPNE / REPNZ
            // 0xf3: REP / REPE / REPZ
            instructions.Add(0xf4, new HLT(0xf4, this, Bus));
            instructions.Add(0xf5, new CMC(0xf5, this, Bus));
            instructions.Add(0xf6, new GRP3(0xf6, this, Bus));
            instructions.Add(0xf7, new GRP3(0xf7, this, Bus));
            instructions.Add(0xf8, new CLC(0xf8, this, Bus));
            instructions.Add(0xf9, new STC(0xf9, this, Bus));
            instructions.Add(0xfa, new CLI(0xfa, this, Bus));
            instructions.Add(0xfb, new STI(0xfb, this, Bus));
            instructions.Add(0xfc, new CLD(0xfc, this, Bus));
            instructions.Add(0xfd, new STD(0xfd, this, Bus));
            instructions.Add(0xfe, new GRP4(0xfe, this, Bus));
            instructions.Add(0xff, new GRP5(0xff, this, Bus));
        }

        public bool TryGetInputDevice(ushort port, out IInputDevice device)
        {
            return (InputDevices.TryGetValue(port, out device));
        }

        public bool TryGetOutputDevice(ushort port, out IOutputDevice device)
        {
            return (OutputDevices.TryGetValue(port, out device));
        }

        public void Interrupt(int interrupt_num)
        {
            var inst = new INT(0, interrupt_num, this, Bus);
            inst.Execute();
        }
    }
}
