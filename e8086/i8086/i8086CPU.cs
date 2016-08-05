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
        // CPU Components
        private i8086ExecutionUnit _eu;
        private i8086BusInterfaceUnit _bus;

        // External chips on 8088
        // Intel 8237
        // Intel 8253 - interrupt timer
        // Intel 8259

        public i8086CPU()
        {

        }

        public void Boot(byte[] program)
        {
            // For now we will hard code the BIOS to start at a particular code segment.
            _bus = new i8086BusInterfaceUnit(0x0000, 0x0000, program);
            _eu = new i8086ExecutionUnit(_bus);

            // i8253 input devices (PIT)
            _eu.AddInputDevice(new i8253(), 0x40);
            _eu.AddInputDevice(new i8253(), 0x41);
            _eu.AddInputDevice(new i8253(), 0x42);
            _eu.AddInputDevice(new i8253(), 0x43);

            // i8253 output devices (PIT)
            _eu.AddOutputDevice(new i8253(), 0x40);
            _eu.AddOutputDevice(new i8253(), 0x41);
            _eu.AddOutputDevice(new i8253(), 0x42);
            _eu.AddOutputDevice(new i8253(), 0x43);

            // i8259 input devices (PIC)
            _eu.AddInputDevice(new i8259(), 0x20);
            _eu.AddInputDevice(new i8259(), 0x21);

            // i8259 output devices (PIC)
            _eu.AddOutputDevice(new i8259(), 0x20);
            _eu.AddOutputDevice(new i8259(), 0x21);

        }

        public long Run()
        {
            long count = 0;
            do
            {
                NextInstruction();
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
