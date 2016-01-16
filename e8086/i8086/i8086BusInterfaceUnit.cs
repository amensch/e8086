using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086BusInterfaceUnit
    {
        public UInt16 CS { get; set; }
        public UInt16 DS { get; set; }
        public UInt16 SS { get; set; }
        public UInt16 ES { get; set; }

        public UInt16 IP { get; set; }
    }
}
