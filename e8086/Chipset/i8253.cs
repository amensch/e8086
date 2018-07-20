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

        The PIT oscillator runs at 1.193182 Mhz.
    */

    public class i8253 
    {

        private enum TimerAccessMode
        {
            Latch = 0,
            LoByte = 1,
            HiByte = 2,
            BothBytes = 3
        };

        private class i8253timer
        {
            public TimerAccessMode AccessMode { get; set; }
            public TimerAccessMode ToggleMode { get; set; }
            public ushort Counter { get; set; }
        }

        private const int TIMERS = 3;

        private i8253timer[] _timers;
        private CPU.InterruptFunc _intFunc;
        private Thread  _timer;
        private bool _stopTimer = false;

        public i8253(CPU.InterruptFunc intFunc)
        {
            //_host_frequency = Stopwatch.Frequency;
            //Active = false;
            _timers = new i8253timer[TIMERS];
            _intFunc = intFunc;
            for( int ii=0; ii < TIMERS; ii++ )
            {
                _timers[ii] = new i8253timer();
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
            if ((_timers[idx].AccessMode == TimerAccessMode.Latch) ||
                (_timers[idx].AccessMode == TimerAccessMode.LoByte) ||
                (_timers[idx].AccessMode == TimerAccessMode.BothBytes && _timers[idx].ToggleMode == TimerAccessMode.LoByte))
            {
                data = (byte)(_timers[idx].Counter & 0x00ff) ;
            }
            else if ((_timers[idx].AccessMode == TimerAccessMode.HiByte) ||
                (_timers[idx].AccessMode == TimerAccessMode.BothBytes && _timers[idx].ToggleMode == TimerAccessMode.HiByte))
            {
                data = (byte)(_timers[idx].Counter >> 8);
            }

            // if toggle mode if access mode is both
            if (_timers[idx].AccessMode == TimerAccessMode.Latch ||
                _timers[idx].AccessMode == TimerAccessMode.BothBytes)
            {
                if (_timers[idx].ToggleMode == TimerAccessMode.LoByte)
                    _timers[idx].ToggleMode = TimerAccessMode.HiByte;
                else
                    _timers[idx].ToggleMode = TimerAccessMode.LoByte;
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
                if (_timer.ThreadState == System.Threading.ThreadState.Running)
                    _stopTimer = true;
            }

            _timers[sc].AccessMode = mode;

            if (mode == TimerAccessMode.Latch ||
                mode == TimerAccessMode.BothBytes)
                _timers[sc].ToggleMode = TimerAccessMode.LoByte;
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
            if( ( _timers[idx].AccessMode == TimerAccessMode.LoByte ) ||
                ( _timers[idx].AccessMode == TimerAccessMode.BothBytes && _timers[idx].ToggleMode == TimerAccessMode.LoByte ))
            {
                // zero out lo byte then assign from data
                _timers[idx].Counter = (ushort)((_timers[idx].Counter & 0xff00) | data);
            }
            else if ((_timers[idx].AccessMode == TimerAccessMode.HiByte) ||
                (_timers[idx].AccessMode == TimerAccessMode.BothBytes && _timers[idx].ToggleMode == TimerAccessMode.HiByte))
            {
                // zero out hi byte then assign from data
                _timers[idx].Counter = (ushort)((_timers[idx].Counter & 0x00ff) | ( data << 8 ));
            }

            // if toggle mode if access mode is both
            if( _timers[idx].AccessMode == TimerAccessMode.BothBytes)
            {
                if( _timers[idx].ToggleMode == TimerAccessMode.LoByte)
                    _timers[idx].ToggleMode = TimerAccessMode.HiByte;
                else
                    _timers[idx].ToggleMode = TimerAccessMode.LoByte;
            }

            // If this timer 1, reset and start timer
            if (idx == 0 &&
                !(_timers[idx].AccessMode == TimerAccessMode.BothBytes &&
                    _timers[idx].ToggleMode == TimerAccessMode.LoByte))
            {
                if(_timer.ThreadState != System.Threading.ThreadState.Running )
                {
                    _timer.Start();
                }
            }
        }

        private void TimerLoop()
        {
            do
            {
                Thread.Sleep(55);
                _intFunc(0);
            } while (!_stopTimer);
            _stopTimer = false;
        }
    }  
}
