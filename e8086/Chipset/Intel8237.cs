using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{

    /*
     * This class is the intel 8237 DMA chip.
0000-001F ----	DMA 1	(first Direct Memory Access controller 8237)

0000	r/w	DMA channel 0  address	byte  0, then byte 1.
0001	r/w	DMA channel 0 word count byte 0, then byte 1.
0002	r/w	DMA channel 1  address	byte  0, then byte 1.
0003	r/w	DMA channel 1 word count byte 0, then byte 1.
0004	r/w	DMA channel 2  address	byte  0, then byte 1.
0005	r/w	DMA channel 2 word count byte 0, then byte 1.
0006	r/w	DMA channel 3  address	byte  0, then byte 1.
0007	r/w	DMA channel 3 word count byte 0, then byte 1.

0008	r	DMA channel 0-3 status register
		 bit 7 = 1  channel 3 request
		 bit 6 = 1  channel 2 request
		 bit 5 = 1  channel 1 request
		 bit 4 = 1  channel 0 request
		 bit 3 = 1  channel terminal count on channel 3
		 bit 2 = 1  channel terminal count on channel 2
		 bit 1 = 1  channel terminal count on channel 1
		 bit 0 = 1  channel terminal count on channel 0

0008	w	DMA channel 0-3 command register
		 bit 7 = 1  DACK sense active high
		       = 0  DACK sense active low
		 bit 6 = 1  DREQ sense active high
		       = 0  DREQ sense active low
		 bit 5 = 1  extended write selection
		       = 0  late write selection
		 bit 4 = 1  rotating priority
		       = 0  fixed priority
		 bit 3 = 1  compressed timing
		       = 0  normal timing
		 bit 2 = 1  enable controller
		       = 0  enable memory-to-memory

0009	w	DMA write request register

000A	r/w	DMA channel 0-3 mask register
		 bit 7-3 = 0   reserved
		 bit 2	 = 0   clear mask bit
			 = 1   set mask bit
		 bit 1-0 = 00  channel 0 select
			 = 01  channel 1 select
			 = 10  channel 2 select
			 = 11  channel 3 select

000B	w	DMA channel 0-3 mode register
		 bit 7-6 = 00  demand mode
			 = 01  single mode
			 = 10  block mode
			 = 11  cascade mode
		 bit 5	 = 0   address increment select
			 = 1   address decrement select
		 bit 3-2 = 00  verify operation
			 = 01  write to memory
			 = 10  read from memory
			 = 11  reserved
		 bit 1-0 = 00  channel 0 select
			 = 01  channel 1 select
			 = 10  channel 2 select
			 = 11  channel 3 select

000C	w	DMA clear byte pointer flip-flop
000D	r	DMA read temporary register
000D	w	DMA master clear
000E	w	DMA clear mask register
000F	w	DMA write mask register

-------------------------------------------------------------------------------
0010-001F ----	DMA controller (8237) on PS/2 model 60 & 80

-------------------------------------------------------------------------------
0018	w	PS/2 extended function register

-------------------------------------------------------------------------------
001A		PS/2 extended function execute

    */
    internal class Intel8237 : IODevice
    {
        private class Intel8237Channel
        {
            public WordRegister Address { get; set; }
            public WordRegister WordCount { get; set; }
            public bool FlipFlop { get; set; }
            public Intel8237Channel()
            {
                Address.Value = 0;
                WordCount.Value = 0;
                FlipFlop = false;
            }
        }

        // 0x00 - 0x07
        private List<Intel8237Channel> Channels = new List<Intel8237Channel>()
        {
            new Intel8237Channel(),
            new Intel8237Channel(),
            new Intel8237Channel(),
            new Intel8237Channel()
        };

        public bool IsListening(ushort port)
        {
            return (port >= 0x00 && port <= 0x1f);
        }

        public int ReadData(int wordSize, ushort port)
        {
            int channelNumber = 0;
            byte data = 0;
            switch(port)
            {
                // Retrieve address word
                case 0x00:
                case 0x02:
                case 0x04:
                case 0x06:
                    {
                        channelNumber = port / 2;
                        var channel = Channels[channelNumber];
                        if(!channel.FlipFlop) // lo byte
                        {
                            data = channel.Address.LO;
                        }
                        else // hi byte
                        {
                            data = channel.Address.HI;
                        }
                        channel.FlipFlop = !channel.FlipFlop;
                        break;
                    }
                // Retrieve word count
                case 0x01:
                case 0x03:
                case 0x05:
                case 0x07:
                    {
                        channelNumber = (port - 1) / 2;
                        var channel = Channels[channelNumber];
                        if (!channel.FlipFlop)
                        {
                            data = channel.WordCount.LO;
                        }
                        else
                        {
                            data = channel.WordCount.HI;
                        }
                        channel.FlipFlop = !channel.FlipFlop;
                        break;
                    }
            }
            return data;
        }

        public void WriteData(int wordSize, ushort port, int data)
        {
            int channelNumber = 0;
            switch (port)
            {
                // Set address word
                case 0x00:
                case 0x02:
                case 0x04:
                case 0x06:
                    {
                        channelNumber = port / 2;
                        var channel = Channels[channelNumber];
                        if (!channel.FlipFlop) // lo byte
                        {
                            channel.Address.LO = (byte)(data & 0xff);
                        }
                        else // hi byte
                        {
                            channel.Address.HI = (byte)(data & 0xff);
                        }
                        channel.FlipFlop = !channel.FlipFlop;
                        break;
                    }
                // Set word count
                case 0x01:
                case 0x03:
                case 0x05:
                case 0x07:
                    {
                        channelNumber = (port - 1) / 2;
                        var channel = Channels[channelNumber];
                        if (!channel.FlipFlop)
                        {
                            channel.WordCount.LO = (byte)(data & 0xff);
                        }
                        else
                        {
                            channel.WordCount.HI = (byte)(data & 0xff);
                        }
                        channel.FlipFlop = !channel.FlipFlop;
                        break;
                    }
            }
        }
    }
}
