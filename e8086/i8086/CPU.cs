using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;

namespace KDS.e8086
{


    public class CPU
    {
        // List of Ports Read on Startup
        /*

            port 0x60: Intel 8042 (keyboard)
            port 0x10: Standard Text Output
            port 0x40: The 8253 timer
                On a real PC interrupt 8 is fired every 55 ms

            OUT 00A0

            OUT 03D8 - CGA mode control register
                        bit 0: 40 vs 80 char mode
                        bit 3: enable video 
                        bit 5: drop # of backcolors to 8 and enable blink
                default value: 0x29 (all on)

            OUT 03B8 - Hercules graphics (HGC) mode control register
                Bit 0 - unused
                    1 - 1=720x348 graphics mode, 0=80x25 text mode
                    2 - unused, should be 0
                    3 - 1=video enabled, 0=video disabled (blank)
                    4 - unused, should be 0
                    5 - 1=blink enabled, 0=blink disabled
                    6 - unused, should be 0
                    7 - 1=graphics memory stars at B800:0000, 0=starts at B000:0000

            OUT 0063
            OUT 0061
            OUT 0043
            OUT 0041
            OUT 0043
            OUT 0081
            OUT 0082
            OUT 0083
            OUT 000D
            OUT 000B
            OUT 000B
            OUT 000B
            OUT 000B
            OUT 0001
            OUT 0001
            OUT 0008
            OUT 000A
            OUT 0043
            OUT 0040
            OUT 0040
            OUT 0213
            OUT 0020
            OUT 0021
            OUT 0021
            OUT 0021
            IN 0061
            OUT 0061
            OUT 0061
            OUT 00A0
            OUT 0061

        */

        public delegate void InterruptFunc(byte int_number);
        public ExecutionUnit EU { get; private set; }
        public BusInterface Bus { get; private set; }

        // External chips
        private Intel8259 PIC;  // interrupts (PIC - Intel8259)
        private Intel8253 PIT;  // timer (PIT - Intel8253)

        // List of interrupts - thread safe FIFO queue
        private ConcurrentQueue<byte> _interrupts;

        public CPU()
        {
            Reset();
        }

        public void Reset()
        {
            _interrupts = new ConcurrentQueue<byte>();
            Bus = new BusInterface();
            Bus.LoadBIOS(File.ReadAllBytes("Chipset\\pcxtbios.bin"));
            //_bus.LoadROM(File.ReadAllBytes("Chipset\\ide_xt.bin"), 0xd0000);
            //_bus.LoadROM(File.ReadAllBytes("Chipset\\rombasic.bin"), 0xf6000);
            //_bus.LoadROM(File.ReadAllBytes("Chipset\\videorom.bin"), 0xc0000);

            EU = new ExecutionUnit(Bus);

            // 0x00 - 0x0f: DMA Chip 8237A-5

            // 0x20 - 0x21: Interrupt 8259A
            PIC = new Intel8259();

            // 0x40 - 0x43: Timer 8253
            PIT = new Intel8253(PIC);

            // 0x60 - 0x63: PPI 8255 (speaker)

            // 0x80 - 0x83: DMA page registers
            // 0xa0 - 0xaf: NMI Mask Register

            // 0x200 - 0x20f: Game Control
            // 0x210 - 0x207: Expansion Unit

            // 0x220 - 0x24f: Reserved

            // 0x3bc: IBM Monochrome Display & Printer Adapter
            // 0x378: Printer Adapter

            EU.AddDevice(PIT);
            EU.AddDevice(PIC);
        }

        public void Boot(byte[] program)
        {
            // For now we will hard code the BIOS to start at a particular code segment.
            Bus = new BusInterface(0x0000, 0x0100, program);
            EU = new ExecutionUnit(Bus);
        }

        public void AddInterrupt(byte int_number)
        {
            _interrupts.Enqueue(int_number);
        }

        public long Run()
        {
            long count = 1;
            byte int_number;
            do
            {

                // check for interrupt
                if( _interrupts.TryDequeue(out int_number))
                {
                    if( EU.CondReg.InterruptEnable )
                    {
                        EU.Interrupt(int_number);
                    }
                }

                NextInstructionDebug();
                count++;
            } while (!EU.Halted);
            return count;
        }

        public void NextInstruction()
        {
            EU.Tick();
        }

        public void NextInstruction(out string dasm)
        {
            Disassembler.DisassembleNext(Bus.GetNext6Bytes(), 0, 0, out dasm);
            NextInstruction();
        }

        public void NextInstructionDebug()
        {
            string dasm;
            Disassembler.DisassembleNext(Bus.GetNext6Bytes(), 0, 0, out dasm);
            Debug.WriteLine(Bus.CS.ToString("X4") + ":" + Bus.IP.ToString("X4") + " " + dasm);
            NextInstruction();
        }



    }
}
