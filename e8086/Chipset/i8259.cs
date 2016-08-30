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

    public class i8259 
    {
        private const byte PIC1_COMMAND = 0x20;
        private const byte PIC1_DATA = 0x21;
        private const byte PIC2_COMMAND = 0xa0;
        private const byte PIC2_DATA = 0xa1;

        private byte _mask;     // imr
        private byte _request;  // irr
        private byte _service;   // isr
        private byte[] _icw = new byte[5];   // icw
        private byte _icwstep;
        private byte _readmode;
        private i8086CPU.InterruptFunc _intFunc;

        public i8259(i8086CPU.InterruptFunc intFunc)
        {
            _intFunc = intFunc;
            _readmode = 0;
            _mask = 0;
            _request = 0;
            _service = 0;
            _icwstep = 0;
        }

        public byte GetNextInterrupt()
        {
            byte temp;

            temp = (byte)(_request & (~_mask));  // XOR request with inverted mask
            for( byte ii = 0; ii < 8; ii++ )
            {
                if( ((temp >> ii) & 0x01) >= 0x01 )
                {
                    _request = (byte)(_request ^ (1 << ii));
                    _service = (byte)(_service | (1 << ii));
                    return ((byte)(_icw[2] + ii));
                }
            }
            return 0;
        }

        // port 0x21
        public byte ReadPicData()
        {
            return _mask;
        }

        // port 0x20
        public byte ReadPicCommand()
        {
            if (_readmode != 0)
            {
                return _service;
            }
            else
            {
                return _request;
            }
        }

        // port 0x21
        public ushort ReadPicData16()
        {
            return ReadPicData();
        }

        // port 0x20
        public ushort ReadPicCommand16()
        {
            return ReadPicCommand();
        }

        // port 0x20
        public void WritePicCommand(byte data)
        {
            if ((byte)(data & 0x10) == 0x10)
            {
                _icwstep = 1;
                _mask = 0;
                _icw[_icwstep++] = data;
            }
            else
            {
                if ((byte)(data & 0x98) == 0x08)
                {
                    if ((byte)(data & 0x02) == 0x02)
                    {
                        _readmode = (byte)(data & 0x02);
                    }
                }
                if ((byte)(data & 0x20) == 0x20)
                {
                    for (int ii = 0; ii < 8; ii++)
                    {
                        if ((byte)((_service >> ii) & 0x01) == 0x01)
                        {
                            _service = (byte)(_service ^ (1 << ii));
                        }
                    }

                }
            }
        }

        // port 0x20
        public void WritePicCommand(ushort data)
        {
            WritePicCommand(data);
        }

        // port 0x21
        public void WritePicData(byte data)
        {
            if ((_icwstep == 3) && ((byte)(_icw[1] & 0x02) == 0x02))
                _icwstep = 4;

            if (_icwstep < 5)
            {
                _icw[_icwstep++] = data;
            }
            else
            {
                _mask = data;
            }
        }

        // port 0x21
        public void WritePicData(ushort data)
        {
            WritePicData(data);
        }
    }
}
