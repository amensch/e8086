using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KDS.e8086
{
    public partial class NumberDialog : Form
    {
        public int Result { get; set; }
        public NumberDialog()
        {
            InitializeComponent();
        }

        public string DialogCaption
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public string TextBoxCaption
        {
            get
            {
                return this.lblCaption.Text;
            }
            set
            {
                this.lblCaption.Text = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return this.txtNumber.Text;
            }
            set
            {
                this.txtNumber.Text = value;
                this.txtNumber.SelectAll();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int num;
            if( int.TryParse(txtNumber.Text, out num))
            {
                Result = num;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(this, "A number is required");
            }
        }
    }
}
