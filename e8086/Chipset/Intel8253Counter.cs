using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public delegate void PITInterruptDelegate();

    internal class Intel8253Counter
    {
        public int Counter { get; private set; }
        public int ResetValue { get; private set; }
        public int LatchValue { get; private set; }
        public byte ControlRegister { get; private set; }
        private bool Latched { get; set; }
        private bool Enabled { get; set; }
        private bool UseLSB { get; set; }

        private bool output;
        private PITInterruptDelegate InterruptHandler;

        public Intel8253Counter()
        {
            Enabled = false;
            output = false;
            Counter = 0;
            ResetValue = 0;
            LatchValue = 0;
            ControlRegister = 0;
        }

        public Intel8253Counter(PITInterruptDelegate interruptHandler) : this()
        {
            InterruptHandler = interruptHandler;
        }

        public bool Output
        {
            get { return output; }
            private set
            {
                // if we are transitioning from low to high then raise the interrupt
                if(!output && value)
                {
                    if(InterruptHandler != null)
                    {
                        InterruptHandler();
                    }
                }
                output = value;
            }
        }
        /*
            Port 0x40: Channel 0 data port
            Port 0x41: Channel 1 data port
            Port 0x42: Channel 2 data port
            Port 0x43: Command register (write only, reads are ignored)

            Each channel has a reload value and the current counter value.


            Port 0x43 command register:
            Bits         Usage
             6 and 7      Select channel :  
                             0 0 = Channel 0
                             0 1 = Channel 1
                             1 0 = Channel 2
                             1 1 = Read-back command (8254 only - AT and later computers)  
             4 and 5      Access mode :    
                             0 0 = Latch count value command
                             0 1 = Access mode: lobyte only
                             1 0 = Access mode: hibyte only
                             1 1 = Access mode: lobyte/hibyte
             1 to 3       Operating mode :
                             0 0 0 = Mode 0 (interrupt on terminal count)
                             0 0 1 = Mode 1 (hardware re-triggerable one-shot)
                             0 1 0 = Mode 2 (rate generator)
                             0 1 1 = Mode 3 (square wave generator)
                             1 0 0 = Mode 4 (software triggered strobe)
                             1 0 1 = Mode 5 (hardware triggered strobe)
                             1 1 0 = Mode 2 (rate generator, same as 010b)
                             1 1 1 = Mode 3 (square wave generator, same as 011b)
             0            BCD/Binary mode: 0 = 16-bit binary, 1 = four-digit BCD 

                Select channel 11 is needed for AT class computers but not implemented in older PCs that use 8253.
                Bit 0 BCD mode is not used in 80x86 computers.  
                Only Operating Modes 0, 2, and 3 are needed for this implementation.
         */

        public void WriteControlRegister(byte data)
        {
            byte access = (byte)((data >> 4) & 0x03);

            // If latch mode, don't change the control register
            // just latch the counter.
            if(access == 0x00)
            {
                Latched = true;
                LatchValue = Counter;
            }
            else
            {
                ControlRegister = data;
            }
        }

        public void LoadCounter(byte data)
        {
            byte access = (byte)((data >> 4) & 0x03);
            byte oper = (byte)((data >> 1) & 0x07);
            switch(access)
            {
                case 0x01:  // LSB only
                    {
                        ResetValue = data & 0xff;
                        break;
                    }
                case 0x02:  // MSB only
                    {
                        ResetValue = (data >> 8) & 0xff;
                        break;
                    }
                case 0x03:  // LSB then MSB
                    {
                        if(UseLSB)
                        {
                            ResetValue = data & 0xff;
                        }
                        else
                        {
                            ResetValue = (data >> 8) & 0xff;
                        }
                        UseLSB = !UseLSB;
                        break;
                    }
            }

            if(access != 0x03 || UseLSB)
            {
                Counter = ResetValue;
                Enabled = true;
                Output = (oper == 0x02 || oper == 0x03 || oper == 0x06 || oper == 0x07);
            }

        }

        public byte ReadCounter()
        {
            byte value = 0;
            int counterValue = Counter;
            byte access = (byte)((ControlRegister >> 4) & 0x03);

            // if the timer is latched override the value with the latched value
            if (Latched)
            {
                counterValue = LatchValue;
                if(access != 0x03 || !UseLSB)
                {
                    Latched = false;
                }
            }
            switch (access)
            {
                case 0x01:  // LSB only
                    {
                        value = (byte)(counterValue & 0xff);
                        break;
                    }
                case 0x02:  // MSB only
                    {
                        value = (byte)(counterValue >> 8 & 0xff);
                        break;
                    }
                case 0x03:  // LSB then MSB
                    {
                        if (UseLSB)
                        {
                            value = (byte)(counterValue & 0xff);
                        }
                        else
                        {
                            value = (byte)(counterValue >> 8 & 0xff);
                        }
                        UseLSB = !UseLSB;
                        break;
                    }
            }

            return value;
        }

        public void ClockPulse()
        {
            if (Enabled)
            {
                byte oper = (byte)((ControlRegister >> 1) & 0x07);
                switch (oper)
                {
                    case 0x00:  // interrupt on count
                        {
                            Counter = (Counter - 1) & 0xffff;
                            if (Counter == 0)
                            {
                                Output = true;
                            }
                            break;
                        }
                    case 0x02:
                    case 0x06:  // rate generator
                        {
                            Counter = (Counter - 1) & 0xffff;
                            if (Counter == 1)
                            {
                                Output = false;
                                Counter = ResetValue;
                            }
                            // set output to high
                            Output = true;
                            break;
                        }
                    case 0x03:
                    case 0x07:  // square wave generator
                        {
                            if (Counter % 2 == 0)
                            {
                                // if the count is even
                                Counter = (Counter - 2) & 0xffff;
                            }
                            else
                            {
                                // if the count is odd
                                if (Output)
                                {
                                    Counter = (Counter - 1) & 0xffff;
                                }
                                else
                                {
                                    Counter = (Counter - 3) & 0xffff;
                                }
                            }

                            // reload count and toggle the output if necessary
                            if (Counter == 0)
                            {
                                Counter = ResetValue;
                                Output = !Output;
                            }
                            break;
                        }
                }
            }
        }
    }
}
