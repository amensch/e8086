using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using KDS.e8086;

namespace CPU_Monitor
{
    public partial class MainForm : Form
    {

        CPU _cpu;

        public MainForm()
        {
            InitializeComponent();

            StackList.DrawItem += StackList_DrawItem;

            Boot();
        }

        private void Boot()
        {
            _cpu = new CPU();
            var program = LoadFile(@"C:\Users\adam\source\repos\e8086\Resources\codegolf.bin");
            _cpu.Boot(0, 0, program);
            _cpu.EU.Registers.SP = 0x0100;
            RefreshData();
        }

        private static byte[] LoadFile(string fileName)
        {
            byte[] buffer;
            FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read);

            try
            {
                int length = (int)fs.Length;
                buffer = new byte[length];
                int bytes_read;
                int total_bytes = 0;

                do
                {
                    bytes_read = fs.Read(buffer, total_bytes, length - total_bytes);
                    total_bytes += bytes_read;
                } while (bytes_read > 0);
            }
            finally
            {
                fs.Close();
            }
            return buffer;
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

            RefreshStackData();
            RefreshDasmData();
            RefreshMemoryData();
        }

        private void RefreshStackData()
        {
            int items_in_list = 26; // this is the number that fits neatly in the size of the listbox
            ushort seg = _cpu.Bus.SS;
            ushort offset = (ushort)(_cpu.EU.Registers.SP - (items_in_list / 2));

            StackList.Items.Clear();

            for( int ii=0; ii < items_in_list; ii++)
            {
                StackList.Items.Add(string.Format("{0:X4}:{1:X4} {2:X2}", seg.ToString("X4"), offset.ToString("X4"), 
                                                                _cpu.Bus.GetDataByPhysical((seg << 4) + offset)));
                offset++;
            }
        }

        private void RefreshDasmData()
        {

        }

        private void RefreshMemoryData()
        {
            StringBuilder sb = new StringBuilder();

            for ( int ii=0; ii < BusInterface.MAX_MEMORY; ii++ )
            {
                sb.Append(string.Format("{0:X2} ", _cpu.Bus.GetDataByPhysical(ii)));
                if ((ii+1) % 16 == 0)
                    sb.AppendLine();
                else if ((ii+1) % 8 == 0)
                    sb.Append("- ");
            }
            MemoryTextBox.Text = sb.ToString();

          
        }

        private Color CondRegColor(bool value)
        {
            if (value)
                return SystemColors.GradientActiveCaption;
            else
                return SystemColors.Window;
        }

        private void StackList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            Graphics g = e.Graphics;
            ListBox lb = (ListBox)sender;

            if (e.Index > -1)
            {
                g.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                g.DrawString(lb.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Black), new PointF(e.Bounds.X, e.Bounds.Y));
            }

            e.DrawFocusRectangle();
        }

        private void RebootButton_Click(object sender, EventArgs e)
        {
            Boot();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {

        }

        private void StopButton_Click(object sender, EventArgs e)
        {

        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            do
            {
                _cpu.NextInstruction();
            } while (_cpu.Bus.IP != 0x0044 && _cpu.Bus.IP != 0x0006);
            RefreshData();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if( keyData == (Keys.F5))
            {
                _cpu.NextInstruction();
                RefreshData();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GoToButton_Click(object sender, EventArgs e)
        {

        }
    }
}
