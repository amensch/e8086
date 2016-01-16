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
using KDS.e8086;
using KDS.Loader;

namespace e8086
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            FileLoader load = new FileLoader();
            //load.AddFile("C:\\Users\\menschas\\Documents\\My Projects\\e8086\\Resources\\vc.com");
            load.AddFile("C:\\Users\\adam\\Documents\\8086\\hexit157\\vc.com");
            Disassemble8086 dasm = new Disassemble8086();
            string output = dasm.Disassemble(load);
        }
    }
}
