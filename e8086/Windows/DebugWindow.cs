using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using KDS.e8086;

namespace KDS.e8086
{
    public partial class DebugWindow : Form
    {

        CPU _cpu;
        string _lastEntry;

        public DebugWindow(CPU cpu)
        {
            InitializeComponent();

            _lastEntry = "";
            _cpu = cpu;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            txtAX.Text = RegToString(_cpu.EU.Registers.AX);
            txtBX.Text = RegToString(_cpu.EU.Registers.BX);
            txtCX.Text = RegToString(_cpu.EU.Registers.CX);
            txtDX.Text = RegToString(_cpu.EU.Registers.DX);
            txtSP.Text = RegToString(_cpu.EU.Registers.SP);
            txtBP.Text = RegToString(_cpu.EU.Registers.BP);
            txtSI.Text = RegToString(_cpu.EU.Registers.SI);
            txtDI.Text = RegToString(_cpu.EU.Registers.DI);
            txtCS.Text = RegToString(_cpu.Bus.CS);
            txtDS.Text = RegToString(_cpu.Bus.DS);
            txtES.Text = RegToString(_cpu.Bus.ES);
            txtSS.Text = RegToString(_cpu.Bus.SS);
            txtIP.Text = RegToString(_cpu.Bus.IP);

            lblTF.BackColor = FlagToColor(_cpu.EU.CondReg.TrapFlag);
            lblDF.BackColor = FlagToColor(_cpu.EU.CondReg.DirectionFlag);
            lblIF.BackColor = FlagToColor(_cpu.EU.CondReg.InterruptEnable);
            lblOF.BackColor = FlagToColor(_cpu.EU.CondReg.OverflowFlag);
            lblSF.BackColor = FlagToColor(_cpu.EU.CondReg.SignFlag);
            lblZF.BackColor = FlagToColor(_cpu.EU.CondReg.ZeroFlag);
            lblAF.BackColor = FlagToColor(_cpu.EU.CondReg.AuxCarryFlag);
            lblPF.BackColor = FlagToColor(_cpu.EU.CondReg.ParityFlag);
            lblCF.BackColor = FlagToColor(_cpu.EU.CondReg.CarryFlag);

            string dasm;
            int bytes = (int)Disassembler.DisassembleNext(_cpu.Bus.GetNext6Bytes(), 0, 0, out dasm);
            lblDasm.Text = dasm;

            lblNext.Text = "";
            byte[] data = _cpu.Bus.GetNextIPBytes(bytes);
            for (int ii = 0; ii < bytes; ii++)
            {
                lblNext.Text += string.Format("{0:X2}", data[ii]);
                lblNext.Text += " ";
            }

            lblInstrCount.Text = _cpu.EU.Stats.InstructionCount.ToString();
        }

        private string RegToString(ushort reg)
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            _cpu.NextInstruction();
            UpdateDisplay();
        }

        private void btnBreakOnIP_Click(object sender, EventArgs e)
        {
            NumberDialog dlg = new NumberDialog();
            dlg.DialogCaption = "Break on IP";
            dlg.TextBoxCaption = "IP:";

            if(dlg.ShowDialog(this) == DialogResult.OK)
            {
                int stopIP = dlg.Result;
                do
                {
                    _cpu.NextInstruction();
                } while (_cpu.Bus.IP != stopIP);
            }
            UpdateDisplay();
        }

        private void btnRunN_Click(object sender, EventArgs e)
        {
            NumberDialog dlg = new NumberDialog();
            dlg.DialogCaption = "Run X Instructions";
            dlg.TextBoxCaption = "Count:";
            dlg.DefaultValue = _lastEntry;

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                long startCount = _cpu.EU.Stats.InstructionCount;
                long stopCount = startCount + dlg.Result;
                do
                {
                    _cpu.NextInstruction();
                } while (_cpu.EU.Stats.InstructionCount < stopCount);
            }
            _lastEntry = dlg.DefaultValue;
            UpdateDisplay();
        }
    }
}
