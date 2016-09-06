using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KDS.e8086;

namespace CPU_Monitor
{
    public partial class MainForm : Form
    {

        i8086CPU _cpu;

        public MainForm()
        {
            InitializeComponent();
            Boot();

            _cpu.EU.CondReg.ParityFlag = true;
            _cpu.EU.Registers.DX = 0xfe37;

            RefreshData();
        }

        private void Boot()
        {
            _cpu = new i8086CPU();
        }

        private void RefreshData()
        {
            CFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.CarryFlag);
            ZFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.ZeroFlag);
            SFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.SignFlag);
            OFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.OverflowFlag);
            PFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.ParityFlag);
            AFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.AuxCarryFlag);
            IFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.InterruptEnable);
            DFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.DirectionFlag);
            TFLabel.BackColor = CondRegColor(_cpu.EU.CondReg.TrapFlag);

            AHLabel.Text = _cpu.EU.Registers.AH.ToString("X2");
            ALLabel.Text = _cpu.EU.Registers.AL.ToString("X2");
            BHLabel.Text = _cpu.EU.Registers.BH.ToString("X2");
            BLLabel.Text = _cpu.EU.Registers.BL.ToString("X2");
            CHLabel.Text = _cpu.EU.Registers.CH.ToString("X2");
            CLLabel.Text = _cpu.EU.Registers.CL.ToString("X2");
            DHLabel.Text = _cpu.EU.Registers.DH.ToString("X2");
            DLLabel.Text = _cpu.EU.Registers.DL.ToString("X2");

            BPLabel.Text = _cpu.EU.Registers.BP.ToString("X4");
            SPLabel.Text = _cpu.EU.Registers.SP.ToString("X4");
            SILabel.Text = _cpu.EU.Registers.SI.ToString("X4");
            DILabel.Text = _cpu.EU.Registers.DI.ToString("X4");

            CSLabel.Text = _cpu.Bus.CS.ToString("X4");
            SSLabel.Text = _cpu.Bus.SS.ToString("X4");
            DSLabel.Text = _cpu.Bus.DS.ToString("X4");
            ESLabel.Text = _cpu.Bus.ES.ToString("X4");

            IPLabel.Text = _cpu.Bus.IP.ToString("X4");

        }

        private Color CondRegColor(bool value)
        {
            if (value)
                return SystemColors.GradientActiveCaption;
            else
                return SystemColors.Window;
        }

    }
}
