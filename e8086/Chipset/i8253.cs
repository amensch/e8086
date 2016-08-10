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

    public class i8253 : IInputDevice, IOutputDevice
    {

        private int _port;

        private const byte MODE_LATCHCOUNT = 0;
        private const byte MODE_LOBYTE = 1;
        private const byte MODE_HIBYTE = 2;
        private const byte MODE_TOGGLE = 3;

        private const byte USE_LO_BYTE = 0;
        private const byte USE_HI_BYTE = 1;

        private const long OSC_FREQUENCY = 1193182;

        private ushort _channeldata;
        private byte _accessmode;
        private byte _bytetoggle;
        private uint _effdata;
        private double _chanfreq;
        private DataRegister16 _counter = new DataRegister16();
        public bool Active { get; set; }

        private long _tick_gap;
        private long _host_frequency;

        public i8253(int port)
        {
            _host_frequency = Stopwatch.Frequency;
            _port = port;
            Active = false;
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
            byte current_byte = 0;

            if( ( _accessmode == MODE_LATCHCOUNT ) ||
                ( _accessmode == MODE_LOBYTE ) ||
                ( ( _accessmode == MODE_TOGGLE ) && _bytetoggle == USE_LO_BYTE ) )
            {
                current_byte = USE_LO_BYTE;
            }
            else if ((_accessmode == MODE_HIBYTE) ||
                ((_accessmode == MODE_TOGGLE) && _bytetoggle == USE_HI_BYTE))
            {
                current_byte = USE_HI_BYTE;
            }


            if ( (_accessmode == MODE_LATCHCOUNT) || (_accessmode == MODE_TOGGLE) )
            {
                // toggle between lo and hi
                _bytetoggle = (byte)((~_bytetoggle) & 0x01);
            }

            if( current_byte == USE_LO_BYTE )
            {
                return _counter.LO;
            }
            else
            {
                return _counter.HI;
            }

        }

        public ushort Read16()
        {
            return Read();
        }

        public void Write(byte data)
        {
            byte current_byte = 0;
            if( _port == 0x43 ) // mode/command register
            {
                _accessmode = (byte)((data >> 4) & 0x03);
                if( _accessmode == MODE_TOGGLE )
                {
                    _bytetoggle = USE_LO_BYTE;
                }
            }
            else
            {
                if( ( _accessmode == MODE_LOBYTE ) ||
                    ( ( _accessmode== MODE_TOGGLE) && ( _bytetoggle == USE_LO_BYTE) ) )
                {
                    current_byte = USE_LO_BYTE;
                }
                else if( ( _accessmode == MODE_HIBYTE ) ||
                    ( ( _accessmode == MODE_TOGGLE ) && ( _bytetoggle == USE_HI_BYTE ) ) )
                {
                    current_byte = USE_HI_BYTE;
                }

                if( current_byte == USE_LO_BYTE)
                {
                    _channeldata = (ushort)((_channeldata & 0xff00) | data);
                }
                else
                {
                    _channeldata = (ushort)((_channeldata & 0x00ff) | data);
                }

                if( _channeldata == 0 )
                {
                    _effdata = 0x10000;
                }
                else
                {
                    _effdata = _channeldata;
                }

                Active = true;

                _tick_gap = _host_frequency / (1193182 / _effdata);
                if( _accessmode == MODE_TOGGLE )
                {
                    // toggle between lo and hi
                    _bytetoggle = (byte)((~_bytetoggle) & 0x01);
                }

                _chanfreq = ((1193182.0 / (double)_effdata) * 1000.0) / 1000.0;
            }
        }

        public void Write16(ushort data)
        {
            Write((byte)data);
        }
    }
}
