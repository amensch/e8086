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
        private i8086ExecutionUnit _eu;

        public i8086CPU()
        {
        }

        public void Boot(byte[] program)
        {
            // For now we will hard code the BIOS to start at a particular code segment.
            _eu = new i8086ExecutionUnit(new i8086BusInterfaceUnit(0x0000, 0x0000, program));
        }

        public void NextInstruction()
        {
            _eu.NextInstruction();
        }

        public void NextInstruction(out string dasm)
        {
            Disassemble8086.DisassembleNext(_eu.Bus.GetNext6Bytes(), 0, 0, out dasm);
            NextInstruction();
        }

        public void NextInstructionDebug()
        {
            string dasm;
            Disassemble8086.DisassembleNext(_eu.Bus.GetNext6Bytes(), 0, 0, out dasm);
            Debug.WriteLine(_eu.Bus.CS.ToString("X4") + ":" + _eu.Bus.IP.ToString("X4") + " " + dasm);
            NextInstruction();
        }

        public i8086ExecutionUnit EU
        {
            get { return _eu; }
        }

    }
}
