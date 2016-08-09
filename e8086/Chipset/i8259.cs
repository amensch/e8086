using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{

    /*
        This class represents the Intel 8259 chip for x86 systems.  On original PCs this was a separate chip.
        Eventually equivalent circuitry is sill included on modern x86 PCs.

        The 8259 is a Programmable Interrupt Controller (PIC).  This chip combines multiple interrupt sources
        into a single interrupt output to the host processor which extends the interrupt levels available beyond
        what the CPU allows.  
        
        Chip / Port
        -------------------------
        Master PIC command - 0x20
        Master PIC data - 0x21
        Slave PIC command - 0xa0
        Slave PIC data - 0xa1

        Real Mode
        -----------------------------
        Master PIC - IRQ 0 to 7, vector offset 0x08, interrupt 0x08 - 0x0f
        Slave PIC - IRQ 8 to 15, vector offset 0x70, interrup 0x70 - 0x77

        NOTE: In original PC/XT only one 8259 chip was used.  
        Beginning with PC/AT two were used in master/slave combination.
    */

    public class i8259 : IInputDevice, IOutputDevice
    {
        private const byte PIC1_COMMAND = 0x20;
        private const byte PIC1_DATA = 0x21;
        private const byte PIC2_COMMAND = 0xa0;
        private const byte PIC2_CDTA = 0xa1;

        private int _port;

        private byte _mask;
        private byte _request;
        private byte _servce;

        public i8259(int port)
        {
            _port = port;
        }

        public int PortNumber
        {
            get
            {
                return _port;
            }
        }

        public byte Read()
        {
            throw new NotImplementedException();
        }

        public ushort Read16()
        {
            throw new NotImplementedException();
        }

        public void Write(byte data)
        {
            throw new NotImplementedException();
        }

        public void Write16(ushort data)
        {
            throw new NotImplementedException();
        }
    }
}
