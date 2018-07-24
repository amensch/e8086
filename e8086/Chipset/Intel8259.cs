using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{

    /*
        This class represents the Intel 8259 chip for x86 systems.  On original PCs this was a separate chip.
        Eventually equivalent circuitry is now integrated with modern x86 PCs.

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

        NOTE: In original PC/XT only one 8259 chip was used (interrupts 0 to 7)
        Beginning with PC/AT two were used in master/slave combination (adding 8 to 15)
    */

    internal class Intel8259 : IODevice
    {
        private const byte PIC1_COMMAND = 0x20;
        private const byte PIC1_DATA = PIC1_COMMAND+1;
        private const byte PIC2_COMMAND = 0xa0;
        private const byte PIC2_DATA = PIC2_COMMAND+1;

        private byte InterruptMask;     // imr
        private byte InterruptRequest;  // irr
        private byte InService;         // isr
        private byte[] ICW = new byte[5];   // icw - Initialization Command Word
        private byte ICWStep;


        public Intel8259()
        {
            InterruptMask = 0;
            InterruptRequest = 0;
            InService = 0;
            ICWStep = 0;
        }

        public bool IsListening(ushort port)
        {
            return (port == PIC1_COMMAND || port == PIC1_DATA || port == PIC2_COMMAND || port == PIC2_DATA);
        }

        public bool HasInterrupt()
        {
            byte mask = (byte)(InterruptRequest & (~InterruptMask));
            return (mask > 0);
        }

        public byte GetNextInterrupt()
        {
            // XOR request with inverted mask
            byte mask = (byte)(InterruptRequest & (~InterruptMask));  

            for( byte ii = 0; ii < 8; ii++ )
            {
                if( ((mask >> ii) & 0x01) >= 0x01 )
                {
                    InterruptRequest = (byte)(InterruptRequest ^ (1 << ii));
                    InService = (byte)(InService | (1 << ii));
                    return ((byte)(ICW[2] + ii));
                }
            }
            return 0;
        }

        public int ReadData(int wordSize, ushort port)
        {
            // this device only returns byte data
            byte data = 0;
            switch (port)
            {
                case PIC1_COMMAND:
                case PIC2_COMMAND:
                    {
                        data = InterruptRequest;
                        break;
                    }
                case PIC1_DATA:
                case PIC2_DATA:
                    {
                        data = InterruptMask;
                        break;
                    }
            }
            return data;
        }

        public void WriteData(int wordSize, ushort port, int data)
        {
            // this device only writes byte data
            byte bytedata = (byte)(data & 0xff);
            switch(port)
            {
                // 0x20, 0xa0
                case PIC1_COMMAND:
                case PIC2_COMMAND:
                    {
                        WritePicCommand(bytedata);
                        break;
                    }

                // 0x21, 0xa1
                case PIC1_DATA:
                case PIC2_DATA:
                    {
                        WritePicData(bytedata);
                        break;
                    }
            }
        }

        private void WritePicCommand(byte data)
        {
            if ((byte)(data & 0x10) == 0x10)
            {
                InterruptMask = 0;
                ICW[ICWStep++] = data;
            }
            else if ((byte)(data & 0x20) == 0x20)
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if ((byte)((InService >> ii) & 0x01) == 0x01)
                    {
                        InService = (byte)(InService ^ (1 << ii));
                    }
                }
            }
        }

        private void WritePicData(byte data)
        {
            if(ICWStep == 1)
            {
                ICW[ICWStep++] = data;
                if((byte)(ICW[0] & 0x02) == 0x02)
                    ICWStep++;
            }
            else if(ICWStep < 4)
            {
                ICW[ICWStep++] = data;
            }
            else
            {
                InterruptMask = data;
            }
        }
    }
}
