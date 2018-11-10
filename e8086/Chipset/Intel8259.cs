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

    internal class Intel8259 : IODevice, IPIC
    {
        private const byte PIC1 = 0x20;
        private const byte PIC2 = 0xa0;

        private const byte PIC1_COMMAND = PIC1;
        private const byte PIC1_DATA = PIC1+1;
        private const byte PIC2_COMMAND = PIC2;
        private const byte PIC2_DATA = PIC2+1;

        /// <summary>
        /// IMR: Interrupt Mask Register
        /// A bitmap of interrupt request lines.  When a bit is set requests are ignored.
        /// Setting this register to 0xff effectively disables the PIC from raising interrupts.
        /// Setting IRQ2 disables the slave PIC.
        /// </summary>
        private byte IMR;     

        /// <summary>
        /// IRR: Interrupt Request Register
        /// A bitmap of which interrupts have been raised.  Once sent to the CPU they are marked in the ISR.
        /// </summary>
        private byte IRR;  

        /// <summary>
        /// ISR: Interrupt Service Register
        /// A bitmap of which interrupts are being serviced (sent to the CPU)
        /// </summary>
        private byte ISR;         

        /// <summary>
        /// ICW: Initialization Command Words
        /// </summary>
        private byte[] ICW = new byte[4];   

        /// <summary>
        /// Initialization step
        /// </summary>
        private byte ICWStep;


        public Intel8259()
        {
            IMR = 0;
            IRR = 0;
            ISR = 0;
            ICWStep = 0;
        }



        #region IO Device Implementation

        public bool IsListening(ushort port)
        {
            return (port == PIC1_COMMAND || port == PIC1_DATA || port == PIC2_COMMAND || port == PIC2_DATA);
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
                        data = IRR;
                        break;
                    }
                case PIC1_DATA:
                case PIC2_DATA:
                    {
                        data = IMR;
                        break;
                    }
            }
            return data;
        }

        public void WriteData(int wordSize, ushort port, int data)
        {
            // this device only writes byte data
            byte bytedata = (byte)(data & 0xff);
            switch (port)
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
        #endregion

        #region IPIC Implementation

        public bool HasInterrupt()
        {
            // IMR disables interrupt if the bit is set
            byte mask = (byte)(IRR & (~IMR));
            return (mask > 0);
        }

        public byte GetNextInterrupt()
        {
            // XOR request with inverted mask
            byte mask = (byte)(IRR & (~IMR));  

            // find the interrupt that is masked
            for( byte ii = 0; ii < 8; ii++ )
            {
                if( ((mask >> ii) & 0x01) >= 0x01 )
                {
                    IRR = (byte)(IRR ^ (1 << ii));
                    ISR = (byte)(ISR | (1 << ii));
                    return ((byte)(ICW[1] + ii));
                }
            }
            return 0;
        }

        public void SetInterrupt(byte irq)
        {
            IRR |= (byte)(0x01 << irq);
        }

        #endregion

        private void WritePicCommand(byte data)
        {
            if ((byte)(data & 0x10) == 0x10)
            {
                IMR = 0;
                ICW[ICWStep++] = data;
            }

            // End of Interrupt (EOI)
            // Program should send this to the PIC when interrupt routine is completed.
            if ((byte)(data & 0x20) == 0x20)
            {
                for (int ii = 0; ii < 8; ii++)
                {
                    if ((byte)((ISR >> ii) & 0x01) == 0x01)
                    {
                        ISR = (byte)(ISR ^ (1 << ii));
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
                IMR = data;
            }
        }
    }
}
