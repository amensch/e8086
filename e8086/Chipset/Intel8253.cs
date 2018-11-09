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

        private Dictionary<ushort, Intel8253Counter> Counters;
        private IPIC PIC;

        public Intel8253(IPIC pic)
        {
            PIC = pic;
            Counters = new Dictionary<ushort, Intel8253Counter>();
            Counters.Add(0x40, new Intel8253Counter(DoInterrupt));
            Counters.Add(0x41, new Intel8253Counter());
            Counters.Add(0x42, new Intel8253Counter());
        }


        #region IODevice Implementation
        public bool IsListening(ushort port)
        {
            return (port >= 0x40 && port <= 0x43);
        }

        public int ReadData(int wordSize, ushort port)
        {
            int value = 0;
            Intel8253Counter counter;
            if(Counters.TryGetValue(port, out counter))
            {
                value = counter.ReadCounter();
            }
            return value;
        }

        public void WriteData(int wordSize, ushort port, int data)
        {
            Intel8253Counter counter;

            // this port is a control word to control the mode of the 3 counters
            if (port == 0x43)
            {
                ushort counterport = (ushort)((data >> 6) & 0x03);
                if (Counters.TryGetValue(counterport, out counter))
                {
                    counter.WriteControlRegister((byte)(data & 0xff));
                }
            }
            else
            {
                if (Counters.TryGetValue(port, out counter))
                {
                    counter.LoadCounter((byte)(data & 0xff));
                }
            }

        }
        #endregion

        public void ClockPulse()
        {
            foreach(var counter in Counters.Values)
            {
                counter.ClockPulse();
            }
        }

        private void DoInterrupt()
        {
            PIC.SetInterrupt(0);
        }
    }
}
