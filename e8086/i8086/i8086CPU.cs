using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KDS.e8086
{
    public class i8086CPU
    {
        // List of Ports Read on Startup
        /*

            OUT 00A0
            OUT 03D8
            OUT 03B8
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

        */


        // CPU Components
        private i8086ExecutionUnit _eu;
        private i8086BusInterfaceUnit _bus;

        // External chips
        private i8259 _i8259;  // interrupts (PIC)
        private i8253 _i8253;  // timer (PIT)

        public i8086CPU()
        {
            Reset();
        }

        public void Reset()
        {
            _bus = new i8086BusInterfaceUnit();
            _bus.LoadBIOS(KDS.Loader.FileLoader.LoadFile("Chipset\\pcxtbios.bin"));

            _eu = new i8086ExecutionUnit(_bus);

            // 0x00 - 0x0f: DMA Chip 8237A-5

            // 0x20 - 0x21: Interrupt 8259A
            Init8259();

            // 0x40 - 0x43: Timer 8253
            Init8253();

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
            _i8259 = new i8259();

            // i8259 input devices (PIC)
            _eu.AddInputDevice(0x20, new InputDevice(_i8259.ReadPicCommand, _i8259.ReadPicCommand16));
            _eu.AddInputDevice(0x21, new InputDevice(_i8259.ReadPicData, _i8259.ReadPicData16));

            // i8259 output devices (PIC)
            _eu.AddOutputDevice(0x20, new OutputDevice(_i8259.WritePicCommand, _i8259.WritePicCommand));
            _eu.AddOutputDevice(0x21, new OutputDevice(_i8259.WritePicData, _i8259.WritePicData));
        }

        private void Init8253()
        {
            _i8253 = new i8253();

            //i8253 input devices(PIT)
            _eu.AddInputDevice(0x40, new InputDevice(_i8253.Read, _i8253.Read16));
            _eu.AddInputDevice(0x41, new InputDevice(_i8253.Read, _i8253.Read16));
            _eu.AddInputDevice(0x42, new InputDevice(_i8253.Read, _i8253.Read16));
            _eu.AddInputDevice(0x43, new InputDevice(_i8253.Read, _i8253.Read16));

            ////i8253 output devices(PIT)
            _eu.AddOutputDevice(0x40, new OutputDevice(_i8253.Write, _i8253.Write));
            _eu.AddOutputDevice(0x41, new OutputDevice(_i8253.Write, _i8253.Write));
            _eu.AddOutputDevice(0x42, new OutputDevice(_i8253.Write, _i8253.Write));
            _eu.AddOutputDevice(0x43, new OutputDevice(_i8253.WritePort43, _i8253.WritePort43));
        }

        public void Boot(byte[] program)
        {
            // For now we will hard code the BIOS to start at a particular code segment.
            _bus = new i8086BusInterfaceUnit(0x0000, 0x0100, program);
            _eu = new i8086ExecutionUnit(_bus);
        }

        public long Run()
        {
            long count = 0;
            do
            {
                NextInstructionDebug();
                count++;
                //Debug.WriteLine("Count: " + count.ToString());
            } while (!_eu.Halted);
            return count;
        }

        public void NextInstruction()
        {
            try
            {
                _eu.NextInstruction();
            }
            catch(Exception ex)
            {
                throw new Exception("Exception thrown", ex);
            }
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
