using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class i8086Registers
    {
        private DataRegister16 _ax = new DataRegister16();
        private DataRegister16 _bx = new DataRegister16();
        private DataRegister16 _cx = new DataRegister16();
        private DataRegister16 _dx = new DataRegister16();

        public ushort SP { get; set; }
        public ushort BP { get; set; }
        public ushort SI { get; set; }
        public ushort DI { get; set; }

        public i8086Registers()
        {
            SP = 0;
            BP = 0;
            SI = 0;
            DI = 0;
        }

        public ushort AX
        {
            get { return _ax.Register; }
            set { _ax.Register = value; }
        }
        public ushort BX
        {
            get { return _bx.Register; }
            set { _bx.Register = value; }
        }
        public ushort CX
        {
            get { return _cx.Register; }
            set { _cx.Register = value; }
        }
        public ushort DX
        {
            get { return _dx.Register; }
            set { _dx.Register = value; }
        }
        public byte AH
        {
            get { return _ax.HI; }
            set { _ax.HI = value; }
        }
        public byte AL
        {
            get { return _ax.LO; }
            set { _ax.LO = value; }
        }
        public byte BH
        {
            get { return _bx.HI; }
            set { _bx.HI = value; }
        }
        public byte BL
        {
            get { return _bx.LO; }
            set { _bx.LO = value; }
        }
        public byte CH
        {
            get { return _cx.HI; }
            set { _cx.HI = value; }
        }
        public byte CL
        {
            get { return _cx.LO; }
            set { _cx.LO = value; }
        }
        public byte DH
        {
            get { return _dx.HI; }
            set { _dx.HI = value; }
        }
        public byte DL
        {
            get { return _dx.LO; }
            set { _dx.LO = value; }
        }
    }
}
