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
        Port 0x43: Command register (write only)
    */

    internal class Intel8253 : IODevice
    {

        private enum TimerAccessMode
        {
            Latch = 0,
            LoByte = 1,
            HiByte = 2,
            BothBytes = 3
        };

        private class Intel8253Timer
        {
            public TimerAccessMode AccessMode { get; set; }
            public TimerAccessMode ToggleMode { get; set; }
            public ushort Counter { get; set; }
            public bool Enabled { get; set; }
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

        public byte ReadCounter1()
        {
            return ReadCounter(0);
        }

        public byte ReadCounter2()
        {
            return ReadCounter(1);
        }

        public byte ReadCounter3()
        {
            return ReadCounter(2);
        }

        public byte ReadControlWord()
        {
            return 0;   // real system ignores this call
        }

        private byte ReadCounter(int idx)
        {
            byte data = 0;
            if ((Timers[idx].AccessMode == TimerAccessMode.Latch) ||
                (Timers[idx].AccessMode == TimerAccessMode.LoByte) ||
                (Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.LoByte))
            {
                data = (byte)(Timers[idx].Counter & 0x00ff) ;
            }
            else if ((Timers[idx].AccessMode == TimerAccessMode.HiByte) ||
                (Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.HiByte))
            {
                data = (byte)(Timers[idx].Counter >> 8);
            }

            // if toggle mode if access mode is both
            if (Timers[idx].AccessMode == TimerAccessMode.Latch ||
                Timers[idx].AccessMode == TimerAccessMode.BothBytes)
            {
                if (Timers[idx].ToggleMode == TimerAccessMode.LoByte)
                    Timers[idx].ToggleMode = TimerAccessMode.HiByte;
                else
                    Timers[idx].ToggleMode = TimerAccessMode.LoByte;
            }

            return data;
        }

        public ushort Read16()
        {
            throw new InvalidOperationException("Not Implemented");
        }

        // port 0x43
        public void WriteControlWord(byte data)
        {
            // control word (bits 7-0)
            // SC1-SC0-RL1-RL0-M2-M1-M0-BCD
            // Set the mode of the timer (0=latch, 1=LSB, 2=MSB, 3=both)

            byte sc = (byte)(data >> 6);
            byte rl = (byte)((data >> 4) & 0x03);
            TimerAccessMode mode = (TimerAccessMode)rl;

            if( sc == 0 )
            {
                //if (_timer.ThreadState == System.Threading.ThreadState.Running)
                //    StopTimer = true;
            }

            Timers[sc].AccessMode = mode;

            if (mode == TimerAccessMode.Latch ||
                mode == TimerAccessMode.BothBytes)
                Timers[sc].ToggleMode = TimerAccessMode.LoByte;
        }

        // port 0x43
        public void WriteControlWord(ushort data)
        {
            WriteControlWord(data);
        }

        public void WriteCounter(ushort data)
        {
            throw new InvalidOperationException("Not Implemented");
        }

        public void WriteCounter1(byte data)
        {
            WriteCounter(0, data);
        }

        public void WriteCounter2(byte data)
        {
            WriteCounter(1, data);
        }

        public void WriteCounter3(byte data)
        {
            WriteCounter(2, data);
        }

        // Write for ports 0x40-0x42
        private void WriteCounter(int idx, byte data)
        {
            if( ( Timers[idx].AccessMode == TimerAccessMode.LoByte ) ||
                ( Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.LoByte ))
            {
                // zero out lo byte then assign from data
                Timers[idx].Counter = (ushort)((Timers[idx].Counter & 0xff00) | data);
            }
            else if ((Timers[idx].AccessMode == TimerAccessMode.HiByte) ||
                (Timers[idx].AccessMode == TimerAccessMode.BothBytes && Timers[idx].ToggleMode == TimerAccessMode.HiByte))
            {
                // zero out hi byte then assign from data
                Timers[idx].Counter = (ushort)((Timers[idx].Counter & 0x00ff) | ( data << 8 ));
            }

            // if toggle mode if access mode is both
            if( Timers[idx].AccessMode == TimerAccessMode.BothBytes)
            {
                if( Timers[idx].ToggleMode == TimerAccessMode.LoByte)
                    Timers[idx].ToggleMode = TimerAccessMode.HiByte;
                else
                    Timers[idx].ToggleMode = TimerAccessMode.LoByte;
            }

            // If this timer 1, reset and start timer
            if (idx == 0 &&
                !(Timers[idx].AccessMode == TimerAccessMode.BothBytes &&
                    Timers[idx].ToggleMode == TimerAccessMode.LoByte))
            {
                //if(_timer.ThreadState != System.Threading.ThreadState.Running )
                //{
                //    _timer.Start();
                //}
            }
        }

        public bool IsListening(ushort port)
        {
            return (port >= 0x40 && port <= 0x43);
        }

        public int ReadData(int wordSize, ushort port)
        {
            throw new NotImplementedException();
        }

        public void WriteData(int wordSize, ushort port, int data)
        {
            throw new NotImplementedException();
        }
    }  
}
