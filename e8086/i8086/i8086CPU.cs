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


    public class i8086CPU
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
        
        // CPU Components
        private i8086ExecutionUnit _eu;
        private i8086BusInterfaceUnit _bus;

        // External chips
        private i8259 _i8259;  // interrupts (PIC)
        private i8253 _i8253;  // timer (PIT)

        // List of interrupts - thread safe FIFO queue
        private ConcurrentQueue<byte> _interrupts;


        public i8086CPU()
        {
            Reset();
        }

        public void Reset()
        {
            _interrupts = new ConcurrentQueue<byte>();
            _bus = new i8086BusInterfaceUnit();
            _bus.LoadBIOS(File.ReadAllBytes("Chipset\\pcxtbios.bin"));
            //_bus.LoadROM(File.ReadAllBytes("Chipset\\ide_xt.bin"), 0xd0000);
            //_bus.LoadROM(File.ReadAllBytes("Chipset\\rombasic.bin"), 0xf6000);
            //_bus.LoadROM(File.ReadAllBytes("Chipset\\videorom.bin"), 0xc0000);

            _eu = new i8086ExecutionUnit(_bus);

            // 0x00 - 0x0f: DMA Chip 8237A-5

            // 0x20 - 0x21: Interrupt 8259A
            Init8259();

            // 0x40 - 0x43: Timer 8253
//            Init8253();

            // 0x60 - 0x63: PPI 8255 (speaker)


            // 0x80 - 0x83: DMA page registers
            // 0xa0 - 0xaf: NMI Mask Register

            // 0x200 - 0x20f: Game Control
            // 0x210 - 0x207: Expansion Unit

            // 0x220 - 0x24f: Reserved

            // 0x3bc: IBM Monochrome Display & Printer Adapter
            // 0x378: Printer Adapter


        }

        private void Init8259()
        {
            _i8259 = new i8259(AddInterrupt);

            // i8259 input devices (PIC)
            _eu.AddInputDevice(0x20, new InputDevice(_i8259.ReadPicCommand, _i8259.ReadPicCommand16));
            _eu.AddInputDevice(0x21, new InputDevice(_i8259.ReadPicData, _i8259.ReadPicData16));

            // i8259 output devices (PIC)
            _eu.AddOutputDevice(0x20, new OutputDevice(_i8259.WritePicCommand, _i8259.WritePicCommand));
            _eu.AddOutputDevice(0x21, new OutputDevice(_i8259.WritePicData, _i8259.WritePicData));
        }

        private void Init8253()
        {
            _i8253 = new i8253(AddInterrupt);

            //i8253 input devices(PIT)
            _eu.AddInputDevice(0x40, new InputDevice(_i8253.ReadCounter1, _i8253.Read16));
            _eu.AddInputDevice(0x41, new InputDevice(_i8253.ReadCounter2, _i8253.Read16));
            _eu.AddInputDevice(0x42, new InputDevice(_i8253.ReadCounter3, _i8253.Read16));
            _eu.AddInputDevice(0x43, new InputDevice(_i8253.ReadControlWord, _i8253.Read16));

            ////i8253 output devices(PIT)
            _eu.AddOutputDevice(0x40, new OutputDevice(_i8253.WriteCounter1, _i8253.WriteCounter));
            _eu.AddOutputDevice(0x41, new OutputDevice(_i8253.WriteCounter2, _i8253.WriteCounter));
            _eu.AddOutputDevice(0x42, new OutputDevice(_i8253.WriteCounter3, _i8253.WriteCounter));
            _eu.AddOutputDevice(0x43, new OutputDevice(_i8253.WriteControlWord, _i8253.WriteControlWord));
        }

        public void Boot(byte[] program)
        {
            // For now we will hard code the BIOS to start at a particular code segment.
            _bus = new i8086BusInterfaceUnit(0x0000, 0x0100, program);
            _eu = new i8086ExecutionUnit(_bus);
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
                    if( _eu.CondReg.InterruptEnable )
                    {
                        _eu.Interrupt(int_number);
                    }
                }

                NextInstructionDebug();
                count++;
            } while (!_eu.Halted);
            return count;
        }

        public void NextInstruction()
        {
            _eu.NextInstruction();
        }

        public void NextInstruction(out string dasm)
        {
            Disassemble8086.DisassembleNext(_bus.GetNext6Bytes(), 0, 0, out dasm);
            NextInstruction();
        }

        public void NextInstructionDebug()
        {
            string dasm;
            Disassemble8086.DisassembleNext(_bus.GetNext6Bytes(), 0, 0, out dasm);
            Debug.WriteLine(_bus.CS.ToString("X4") + ":" + _bus.IP.ToString("X4") + " " + dasm);
            NextInstruction();
        }

        public i8086ExecutionUnit EU
        {
            get { return _eu; }
        }

        public i8086BusInterfaceUnit Bus
        {
            get { return _bus; }
        }

    }
}
