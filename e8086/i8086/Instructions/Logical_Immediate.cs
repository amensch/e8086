using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class Logical_Immediate : Logical
    {
        public Logical_Immediate(byte opCode, IExecutionUnit eu, IBus bus) : base(opCode, eu, bus) { }

        protected override void ExecuteInstruction()
        {
            int source;
            if (wordSize == 0)
            {
                source = secondByte.Value;
            }
            else
            {
                byte lo = secondByte.Value;
                byte hi = Bus.NextIP();
                source = new DataRegister16(hi, lo);
            }

            // override default values
            // direction always = 0
            // mod/reg/rm set to use AL/AX and immediate
            direction = 0;
            byte mod = 0x03;
            byte rm = 0x00;
            byte reg = 0x00;

            // test op code to determine the operator
            switch ((byte)(OpCode >> 4))
            {
                case 0x00: // OR
                    {
                        OR_Destination(source, direction, wordSize, mod, reg, rm);
                        break;
                    }
                case 0x02: // AND
                    {
                        AND_Destination(source, direction, wordSize, mod, reg, rm, false);
                        break;
                    }
                case 0x03: // XOR
                    {
                        XOR_Destination(source, direction, wordSize, mod, reg, rm);
                        break;
                    }
                case 0x08: // TEST
                    {
                        AND_Destination(source, direction, wordSize, mod, reg, rm, true);
                        break;
                    }
            }
        }
    }


}
