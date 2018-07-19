using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDS.e8086
{
    public class Registers
    {
        private WordRegister _ax = new WordRegister();
        private WordRegister _bx = new WordRegister();
        private WordRegister _cx = new WordRegister();
        private WordRegister _dx = new WordRegister();

        public ushort SP { get; set; }
        public ushort BP { get; set; }
        public ushort SI { get; set; }
        public ushort DI { get; set; }

        public Registers()
        {
            SP = 0;
            BP = 0;
            SI = 0;
            DI = 0;
        }

        public ushort AX
        {
            get { return _ax.Value; }
            set { _ax.Value = value; }
        }
        public ushort BX
        {
            get { return _bx.Value; }
            set { _bx.Value = value; }
        }
        public ushort CX
        {
            get { return _cx.Value; }
            set { _cx.Value = value; }
        }
        public ushort DX
        {
            get { return _dx.Value; }
            set { _dx.Value = value; }
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
