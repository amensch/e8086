using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KDS.e8086
{
    /*
        This class represents the Intel 8253 chip for x86 systems.  On original PCs this was a separate chip.
        Eventually equivalent circuitry is sill included on modern x86 PCs.

        The 8253 is a Programmable Interval Timer (PIT).

        Port 0x40: Channel 0 data port
        Port 0x41: Channel 1 data port
        Port 0x42: Channel 2 data port
        Port 0x43: Command register (write only, reads are ignored)

        Each input pulse causes the counter to decrement. When counter is zero an output pulse is generated and the counter reset.
        The counter has different modes to specific what action to take when the counter reaches zero.
            - a frequency divider where the counter is reset
            - a one shot timer where the count isn't reset

        Output channels:
            Channel 0 is connected directly to PIC IRQ 0.  
            During boot the BIOS sets channel 0 with a count of 65535 or 0 and gives and output frequency of 18.2065 hz (the lowest setting).

            Channel 1 was once used for refreshing RAM.  This behavior is no longer needed in modern computers.

            Channel 2 is connected to the PC speaker.  The frequency of output translates to the frequency of sound.
    */

    internal class Intel8253 : IODevice
    {

        private enum TimerAccessMode
        {
            Latch = 0,      // toggle latch
            LoByte = 1,     // read lo byte
            HiByte = 2,     // read hi byte
            BothBytes = 3   // read lo then hi
        };

        private class Intel8253Timer
        {
            /// <summary>
            /// Current mode of this counter
            /// </summary>
            public TimerAccessMode AccessMode { get; set; }

            /// <summary>
            ///  The actual counter value
            /// </summary>
            public int Counter { get; set; }

            /// <summary>
            /// Reset value of the counter
            /// </summary>
            public int ResetValue { get; set; }

            /// <summary>
            /// Latched value of the counter
            /// </summary>
            public int LatchValue { get; set; }

            /// <summary>
            /// Raw byte value of the control word for this timer
            /// </summary>
            public byte ControlWord { get; set; }

            /// <summary>
            /// Toggle mode for reading and writing both LSB and MSB
            /// True = read/write LSB
            /// False = read/write MSB
            /// </summary>
            public bool UseLSB { get; set; }

            /// <summary>
            /// The timers begin disabled and are enabled when used
            /// </summary>
            public bool Enabled { get; set; } = false;

            /// <summary>
            /// Indicated if the timer is currently latched
            /// </summary>
            public bool Latched { get; set; }
        }

        private const int TIMERS = 3;

        private Intel8253Timer[] Timers;
        private bool StopTimer = false;
        private IPIC PIC;

        public Intel8253(IPIC pic)
        {
            PIC = pic;
            Timers = new Intel8253Timer[TIMERS];
            for( int ii=0; ii < TIMERS; ii++ )
            {
                Timers[ii] = new Intel8253Timer();
            }
        }


        #region IODevice Implementation
        public bool IsListening(ushort port)
        {
            return (port >= 0x40 && port <= 0x43);
        }

        public int ReadData(int wordSize, ushort port)
        {
            int value = 0;
            int idx = port & 0x03;

            if (port < 0x40 || port > 0x42) return value;

            var timer = Timers[idx];
            value = timer.Counter;
            if(timer.Latched)
            {
                value = timer.LatchValue;
                if((timer.AccessMode != TimerAccessMode.BothBytes) || timer.UseLSB)
                {
                    timer.Latched = false;
                }
            }

            switch(timer.AccessMode)
            {
                case TimerAccessMode.Latch:
                    {
                        break;
                    }
                case TimerAccessMode.LoByte:
                    {
                        value = value & 0xff;
                        break;
                    }
                case TimerAccessMode.HiByte:
                    {
                        value = (value >> 8) & 0xff;
                        break;
                    }
                case TimerAccessMode.BothBytes:
                    {
                        if(timer.UseLSB)
                        {
                            value = value & 0xff;
                        }
                        else
                        {
                            value = (value >> 8) & 0xff;
                        }
                        timer.UseLSB = !timer.UseLSB;
                        break;
                    }
            }
            
            return value;
        }

        public void WriteData(int wordSize, ushort port, int data)
        {
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

            // Only byte data is written

            if(port == 0x43)
            {
                // write the control word
            }
            else
            {
                int idx = port & 0x03;
                var timer = Timers[idx];

                switch (timer.AccessMode)
                {
                    case TimerAccessMode.Latch:
                        {
                            break;
                        }
                    case TimerAccessMode.LoByte:
                        {
                            
                            break;
                        }
                    case TimerAccessMode.HiByte:
                        {
                            break;
                        }
                    case TimerAccessMode.BothBytes:
                        {
                            break;
                        }
                }
            }

        }
        #endregion

        public void Clock()
        {
            // loop through each timer and increment as necessary
            foreach (var timer in Timers)
            {

            }
        }

        //public byte ReadCounter1()
        //{
        //    return ReadCounter(0);
        //}

        //public byte ReadCounter2()
        //{
        //    return ReadCounter(1);
        //}

        //public byte ReadCounter3()
        //{
        //    return ReadCounter(2);
        //}

        //public byte ReadControlWord()
        //{
        //    return 0;   // real system ignores this call
        //}

        //private byte ReadCounter(int idx)
        //{
        //    byte data = 0;

        //    if ((Timers[idx].AccessMode == TimerAccessMode.Latch) ||
        //        (Timers[idx].AccessMode == TimerAccessMode.LoByte) ||
        //        (Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.LoByte))
        //    {
        //        data = (byte)(Timers[idx].Counter & 0x00ff) ;
        //    }
        //    else if ((Timers[idx].AccessMode == TimerAccessMode.HiByte) ||
        //        (Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.HiByte))
        //    {
        //        data = (byte)(Timers[idx].Counter >> 8);
        //    }

        //    // if toggle mode if access mode is both
        //    if (Timers[idx].AccessMode == TimerAccessMode.Latch ||
        //        Timers[idx].AccessMode == TimerAccessMode.BothBytes)
        //    {
        //        if (Timers[idx].ToggleMode == TimerAccessMode.LoByte)
        //            Timers[idx].ToggleMode = TimerAccessMode.HiByte;
        //        else
        //            Timers[idx].ToggleMode = TimerAccessMode.LoByte;
        //    }

        //    return data;
        //}

        //// port 0x43
        //private void WriteControlWord(byte data)
        //{
        //    // control word (bits 7-0)
        //    // SC1-SC0-RL1-RL0-M2-M1-M0-BCD
        //    // Set the mode of the timer (0=latch, 1=LSB, 2=MSB, 3=both)

        //    byte sc = (byte)(data >> 6);
        //    byte rl = (byte)((data >> 4) & 0x03);
        //    TimerAccessMode mode = (TimerAccessMode)rl;

        //    Timers[sc].AccessMode = mode;

        //    if (mode == TimerAccessMode.Latch || mode == TimerAccessMode.BothBytes)
        //    {
        //        Timers[sc].ToggleMode = TimerAccessMode.LoByte;
        //    }
        //}

        //public void WriteCounter(ushort data)
        //{
        //    throw new InvalidOperationException("Not Implemented");
        //}

        //public void WriteCounter1(byte data)
        //{
        //    WriteCounter(0, data);
        //}

        //public void WriteCounter2(byte data)
        //{
        //    WriteCounter(1, data);
        //}

        //public void WriteCounter3(byte data)
        //{
        //    WriteCounter(2, data);
        //}

        //// Write for ports 0x40-0x42
        //private void WriteCounter(int idx, byte data)
        //{
        //    if( ( Timers[idx].AccessMode == TimerAccessMode.LoByte ) ||
        //        ( Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.LoByte ))
        //    {
        //        // zero out lo byte then assign from data
        //        Timers[idx].Counter = (ushort)((Timers[idx].Counter & 0xff00) | data);
        //    }
        //    else if ((Timers[idx].AccessMode == TimerAccessMode.HiByte) ||
        //        (Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.HiByte))
        //    {
        //        // zero out hi byte then assign from data
        //        Timers[idx].Counter = (ushort)((Timers[idx].Counter & 0x00ff) | ( data << 8 ));
        //    }

        //    // if toggle mode if access mode is both
        //    if( Timers[idx].AccessMode == TimerAccessMode.BothBytes)
        //    {
        //        if( Timers[idx].ToggleMode == TimerAccessMode.LoByte)
        //            Timers[idx].ToggleMode = TimerAccessMode.HiByte;
        //        else
        //            Timers[idx].ToggleMode = TimerAccessMode.LoByte;
        //    }

        //    // If this timer 1, reset and start timer
        //    if (idx == 0 &&
        //        !(Timers[idx].AccessMode == TimerAccessMode.BothBytes &&
        //            Timers[idx].ToggleMode == TimerAccessMode.LoByte))
        //    {
        //        //if(_timer.ThreadState != System.Threading.ThreadState.Running )
        //        //{
        //        //    _timer.Start();
        //        //}
        //    }
        //}


    }  
}
