using System;
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
        private class i8253Timer
        {
            private byte _port;
            private DataRegister16 _counter;
            private byte _ctlword;
            private bool _enabled;

            public i8253Timer(byte port)
            {
                _port = port;
                _counter = new DataRegister16();
                _enabled = false;
            }

            public byte Port
            {
                get { return _port; }
            }

        }

        // In hertz.  In practical terms, a full turn through the 16 bit counter
        // results in an interrupt roughly every 55 ms.
        private const long OSC_FREQUENCY = 1193182;

        private const int TIMERS = 3;
        private long _hostTicksPerSecond;



        private DataRegister16[] _counters = new DataRegister16[TIMERS];
        private byte[] _ctlword = new byte[TIMERS];
        private bool[] _enabled = new bool[TIMERS];

        private byte _databus;

        public i8253()
        {
            _hostTicksPerSecond = Stopwatch.Frequency;
            for( int ii = 0; ii < TIMERS; ii++ )
            {
                _enabled[ii] = false;
            }
        }



    }
}
