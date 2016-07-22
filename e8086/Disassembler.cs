using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using KDS.e8086;
using KDS.Loader;

namespace e8086
{
    public partial class Form1 : Form
    {

        i8086CPU cpu = new i8086CPU();

        public Form1()
        {
            InitializeComponent();

            FileLoader load = new FileLoader();
            cpu.Boot(load.LoadFile("C:\\Users\\adam\\Documents\\my projects\\e8086\\resources\\codegolf.bin"));
            cpu.EU.Registers.SP = 0x100;

            UpdateDisplay();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            cpu.NextInstruction();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            txtAX.Text = RegToString(cpu.EU.Registers.AX);
            txtBX.Text = RegToString(cpu.EU.Registers.BX);
            txtCX.Text = RegToString(cpu.EU.Registers.CX);
            txtDX.Text = RegToString(cpu.EU.Registers.DX);
            txtSP.Text = RegToString(cpu.EU.Registers.SP);
            txtBP.Text = RegToString(cpu.EU.Registers.BP);
            txtSI.Text = RegToString(cpu.EU.Registers.SI);
            txtDI.Text = RegToString(cpu.EU.Registers.DI);
            txtCS.Text = RegToString(cpu.EU.Bus.CS);
            txtDS.Text = RegToString(cpu.EU.Bus.DS);
            txtES.Text = RegToString(cpu.EU.Bus.ES);
            txtSS.Text = RegToString(cpu.EU.Bus.SS);
            txtIP.Text = RegToString(cpu.EU.Bus.IP);

            lblTF.BackColor = FlagToColor(cpu.EU.CondReg.TrapFlag);
            lblDF.BackColor = FlagToColor(cpu.EU.CondReg.DirectionFlag);
            lblIF.BackColor = FlagToColor(cpu.EU.CondReg.InterruptEnable);
            lblOF.BackColor = FlagToColor(cpu.EU.CondReg.OverflowFlag);
            lblSF.BackColor = FlagToColor(cpu.EU.CondReg.SignFlag);
            lblZF.BackColor = FlagToColor(cpu.EU.CondReg.ZeroFlag);
            lblAF.BackColor = FlagToColor(cpu.EU.CondReg.AuxCarryFlag);
            lblPF.BackColor = FlagToColor(cpu.EU.CondReg.ParityFlag);
            lblCF.BackColor = FlagToColor(cpu.EU.CondReg.CarryFlag);

            string dasm;
            int bytes = (int)Disassemble8086.DisassembleNext(cpu.EU.Bus.GetNext6Bytes(), 0, 0, out dasm);
            txtDasm.Text = dasm;

            txtNextIP.Text = "";
            byte[] data = cpu.EU.Bus.GetNextIPBytes(bytes);
            for( int ii = 0; ii < bytes; ii++ )
            {
                txtNextIP.Text += string.Format("{0:X2}", data[ii]);
                txtNextIP.Text += " ";
            }

        }

        private string RegToString(UInt16 reg)
        {
            return string.Format("{0:X4}", reg);
        }

        private Color FlagToColor(bool flag)
        {
            if (flag)
                return Color.PaleGreen;
            else
                return SystemColors.Control;
        }

        private void btnKeepGoing_Click(object sender, EventArgs e)
        {
            string dasm;
            for(int ii = 0; ii < 173; ii++ )
            {
                Disassemble8086.DisassembleNext(cpu.EU.Bus.GetNext6Bytes(), 0, 0, out dasm);
                Debug.WriteLine(string.Format("{0:X4}: {1}", cpu.EU.Bus.IP, dasm));
                btnNext.PerformClick();

                if(cpu.EU.Bus.IP > 0x01d5)
                {
                    Debug.WriteLine("IP is too big!!! ii={0}",ii);
                    break;
                }
            }
            UpdateDisplay();
        }
    }
}
