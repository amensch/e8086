namespace CPU_Monitor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FlagsGroupBox = new System.Windows.Forms.GroupBox();
            this.TFLabel = new System.Windows.Forms.Label();
            this.DFLabel = new System.Windows.Forms.Label();
            this.IFLabel = new System.Windows.Forms.Label();
            this.AFLabel = new System.Windows.Forms.Label();
            this.PFLabel = new System.Windows.Forms.Label();
            this.OFLabel = new System.Windows.Forms.Label();
            this.SFLabel = new System.Windows.Forms.Label();
            this.ZFLabel = new System.Windows.Forms.Label();
            this.CFLabel = new System.Windows.Forms.Label();
            this.RegistersGroupBox = new System.Windows.Forms.GroupBox();
            this.DILabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.SILabel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.SPLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.IPLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BPLabel = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.ESLabel = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.DSLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SSLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.CSLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DLLabel = new System.Windows.Forms.Label();
            this.DHLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.CLLabel = new System.Windows.Forms.Label();
            this.CHLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BLLabel = new System.Windows.Forms.Label();
            this.BHLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ALLabel = new System.Windows.Forms.Label();
            this.AHLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.StackGroupBox = new System.Windows.Forms.GroupBox();
            this.StackList = new System.Windows.Forms.ListBox();
            this.DasmList = new System.Windows.Forms.CheckedListBox();
            this.MemoryTextBox = new System.Windows.Forms.TextBox();
            this.RebootButton = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.RunButton = new System.Windows.Forms.Button();
            this.txtSegment = new System.Windows.Forms.TextBox();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.GoToButton = new System.Windows.Forms.Button();
            this.FlagsGroupBox.SuspendLayout();
            this.RegistersGroupBox.SuspendLayout();
            this.StackGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FlagsGroupBox
            // 
            this.FlagsGroupBox.Controls.Add(this.TFLabel);
            this.FlagsGroupBox.Controls.Add(this.DFLabel);
            this.FlagsGroupBox.Controls.Add(this.IFLabel);
            this.FlagsGroupBox.Controls.Add(this.AFLabel);
            this.FlagsGroupBox.Controls.Add(this.PFLabel);
            this.FlagsGroupBox.Controls.Add(this.OFLabel);
            this.FlagsGroupBox.Controls.Add(this.SFLabel);
            this.FlagsGroupBox.Controls.Add(this.ZFLabel);
            this.FlagsGroupBox.Controls.Add(this.CFLabel);
            this.FlagsGroupBox.Location = new System.Drawing.Point(13, 13);
            this.FlagsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.FlagsGroupBox.Name = "FlagsGroupBox";
            this.FlagsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.FlagsGroupBox.Size = new System.Drawing.Size(119, 375);
            this.FlagsGroupBox.TabIndex = 0;
            this.FlagsGroupBox.TabStop = false;
            this.FlagsGroupBox.Text = "Flags";
            // 
            // TFLabel
            // 
            this.TFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.TFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TFLabel.Location = new System.Drawing.Point(8, 334);
            this.TFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TFLabel.Name = "TFLabel";
            this.TFLabel.Size = new System.Drawing.Size(100, 28);
            this.TFLabel.TabIndex = 8;
            this.TFLabel.Text = "TF: Trap";
            this.TFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DFLabel
            // 
            this.DFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.DFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DFLabel.Location = new System.Drawing.Point(8, 294);
            this.DFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DFLabel.Name = "DFLabel";
            this.DFLabel.Size = new System.Drawing.Size(100, 28);
            this.DFLabel.TabIndex = 7;
            this.DFLabel.Text = "DF: Direction";
            this.DFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IFLabel
            // 
            this.IFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.IFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.IFLabel.Location = new System.Drawing.Point(8, 255);
            this.IFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.IFLabel.Name = "IFLabel";
            this.IFLabel.Size = new System.Drawing.Size(100, 28);
            this.IFLabel.TabIndex = 6;
            this.IFLabel.Text = "IF: Interrupt";
            this.IFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AFLabel
            // 
            this.AFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.AFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AFLabel.Location = new System.Drawing.Point(8, 216);
            this.AFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AFLabel.Name = "AFLabel";
            this.AFLabel.Size = new System.Drawing.Size(100, 28);
            this.AFLabel.TabIndex = 5;
            this.AFLabel.Text = "AF: Aux Carry";
            this.AFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PFLabel
            // 
            this.PFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.PFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PFLabel.Location = new System.Drawing.Point(8, 176);
            this.PFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PFLabel.Name = "PFLabel";
            this.PFLabel.Size = new System.Drawing.Size(100, 28);
            this.PFLabel.TabIndex = 4;
            this.PFLabel.Text = "PF: Parity";
            this.PFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OFLabel
            // 
            this.OFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.OFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.OFLabel.Location = new System.Drawing.Point(8, 137);
            this.OFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.OFLabel.Name = "OFLabel";
            this.OFLabel.Size = new System.Drawing.Size(100, 28);
            this.OFLabel.TabIndex = 3;
            this.OFLabel.Text = "OF: Overflow";
            this.OFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SFLabel
            // 
            this.SFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.SFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SFLabel.Location = new System.Drawing.Point(8, 97);
            this.SFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SFLabel.Name = "SFLabel";
            this.SFLabel.Size = new System.Drawing.Size(100, 28);
            this.SFLabel.TabIndex = 2;
            this.SFLabel.Text = "SF: Sign";
            this.SFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ZFLabel
            // 
            this.ZFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.ZFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ZFLabel.Location = new System.Drawing.Point(8, 58);
            this.ZFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ZFLabel.Name = "ZFLabel";
            this.ZFLabel.Size = new System.Drawing.Size(100, 28);
            this.ZFLabel.TabIndex = 1;
            this.ZFLabel.Text = "ZF: Zero";
            this.ZFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CFLabel
            // 
            this.CFLabel.BackColor = System.Drawing.SystemColors.Control;
            this.CFLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CFLabel.Location = new System.Drawing.Point(8, 19);
            this.CFLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CFLabel.Name = "CFLabel";
            this.CFLabel.Size = new System.Drawing.Size(100, 28);
            this.CFLabel.TabIndex = 0;
            this.CFLabel.Text = "CF: Carry";
            this.CFLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RegistersGroupBox
            // 
            this.RegistersGroupBox.Controls.Add(this.DILabel);
            this.RegistersGroupBox.Controls.Add(this.label17);
            this.RegistersGroupBox.Controls.Add(this.SILabel);
            this.RegistersGroupBox.Controls.Add(this.label15);
            this.RegistersGroupBox.Controls.Add(this.SPLabel);
            this.RegistersGroupBox.Controls.Add(this.label11);
            this.RegistersGroupBox.Controls.Add(this.IPLabel);
            this.RegistersGroupBox.Controls.Add(this.label5);
            this.RegistersGroupBox.Controls.Add(this.BPLabel);
            this.RegistersGroupBox.Controls.Add(this.label14);
            this.RegistersGroupBox.Controls.Add(this.ESLabel);
            this.RegistersGroupBox.Controls.Add(this.label12);
            this.RegistersGroupBox.Controls.Add(this.DSLabel);
            this.RegistersGroupBox.Controls.Add(this.label9);
            this.RegistersGroupBox.Controls.Add(this.SSLabel);
            this.RegistersGroupBox.Controls.Add(this.label6);
            this.RegistersGroupBox.Controls.Add(this.CSLabel);
            this.RegistersGroupBox.Controls.Add(this.label3);
            this.RegistersGroupBox.Controls.Add(this.DLLabel);
            this.RegistersGroupBox.Controls.Add(this.DHLabel);
            this.RegistersGroupBox.Controls.Add(this.label10);
            this.RegistersGroupBox.Controls.Add(this.CLLabel);
            this.RegistersGroupBox.Controls.Add(this.CHLabel);
            this.RegistersGroupBox.Controls.Add(this.label7);
            this.RegistersGroupBox.Controls.Add(this.BLLabel);
            this.RegistersGroupBox.Controls.Add(this.BHLabel);
            this.RegistersGroupBox.Controls.Add(this.label4);
            this.RegistersGroupBox.Controls.Add(this.ALLabel);
            this.RegistersGroupBox.Controls.Add(this.AHLabel);
            this.RegistersGroupBox.Controls.Add(this.label1);
            this.RegistersGroupBox.Location = new System.Drawing.Point(140, 13);
            this.RegistersGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.RegistersGroupBox.Name = "RegistersGroupBox";
            this.RegistersGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.RegistersGroupBox.Size = new System.Drawing.Size(198, 375);
            this.RegistersGroupBox.TabIndex = 1;
            this.RegistersGroupBox.TabStop = false;
            this.RegistersGroupBox.Text = "Registers";
            // 
            // DILabel
            // 
            this.DILabel.BackColor = System.Drawing.SystemColors.Window;
            this.DILabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DILabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DILabel.Location = new System.Drawing.Point(124, 295);
            this.DILabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DILabel.Name = "DILabel";
            this.DILabel.Size = new System.Drawing.Size(63, 28);
            this.DILabel.TabIndex = 29;
            this.DILabel.Text = "0000";
            this.DILabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(102, 302);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(23, 13);
            this.label17.TabIndex = 28;
            this.label17.Text = "DI";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SILabel
            // 
            this.SILabel.BackColor = System.Drawing.SystemColors.Window;
            this.SILabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SILabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SILabel.Location = new System.Drawing.Point(124, 256);
            this.SILabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SILabel.Name = "SILabel";
            this.SILabel.Size = new System.Drawing.Size(63, 28);
            this.SILabel.TabIndex = 27;
            this.SILabel.Text = "0000";
            this.SILabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(102, 262);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "SI";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SPLabel
            // 
            this.SPLabel.BackColor = System.Drawing.SystemColors.Window;
            this.SPLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SPLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SPLabel.Location = new System.Drawing.Point(124, 217);
            this.SPLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SPLabel.Name = "SPLabel";
            this.SPLabel.Size = new System.Drawing.Size(63, 28);
            this.SPLabel.TabIndex = 25;
            this.SPLabel.Text = "0000";
            this.SPLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(102, 223);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(23, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "SP";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IPLabel
            // 
            this.IPLabel.BackColor = System.Drawing.SystemColors.Window;
            this.IPLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.IPLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IPLabel.Location = new System.Drawing.Point(124, 177);
            this.IPLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(63, 28);
            this.IPLabel.TabIndex = 23;
            this.IPLabel.Text = "0000";
            this.IPLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(102, 183);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "IP";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BPLabel
            // 
            this.BPLabel.BackColor = System.Drawing.SystemColors.Window;
            this.BPLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BPLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BPLabel.Location = new System.Drawing.Point(31, 334);
            this.BPLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BPLabel.Name = "BPLabel";
            this.BPLabel.Size = new System.Drawing.Size(63, 28);
            this.BPLabel.TabIndex = 21;
            this.BPLabel.Text = "0000";
            this.BPLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 340);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(26, 16);
            this.label14.TabIndex = 20;
            this.label14.Text = "BP";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ESLabel
            // 
            this.ESLabel.BackColor = System.Drawing.SystemColors.Window;
            this.ESLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ESLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ESLabel.Location = new System.Drawing.Point(31, 294);
            this.ESLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ESLabel.Name = "ESLabel";
            this.ESLabel.Size = new System.Drawing.Size(63, 28);
            this.ESLabel.TabIndex = 19;
            this.ESLabel.Text = "0000";
            this.ESLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 301);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 16);
            this.label12.TabIndex = 18;
            this.label12.Text = "ES";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DSLabel
            // 
            this.DSLabel.BackColor = System.Drawing.SystemColors.Window;
            this.DSLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DSLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DSLabel.Location = new System.Drawing.Point(31, 255);
            this.DSLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DSLabel.Name = "DSLabel";
            this.DSLabel.Size = new System.Drawing.Size(63, 28);
            this.DSLabel.TabIndex = 17;
            this.DSLabel.Text = "0000";
            this.DSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 261);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 16);
            this.label9.TabIndex = 16;
            this.label9.Text = "DS";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SSLabel
            // 
            this.SSLabel.BackColor = System.Drawing.SystemColors.Window;
            this.SSLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SSLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SSLabel.Location = new System.Drawing.Point(31, 216);
            this.SSLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SSLabel.Name = "SSLabel";
            this.SSLabel.Size = new System.Drawing.Size(63, 28);
            this.SSLabel.TabIndex = 15;
            this.SSLabel.Text = "0000";
            this.SSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 222);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "SS";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CSLabel
            // 
            this.CSLabel.BackColor = System.Drawing.SystemColors.Window;
            this.CSLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CSLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CSLabel.Location = new System.Drawing.Point(31, 176);
            this.CSLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CSLabel.Name = "CSLabel";
            this.CSLabel.Size = new System.Drawing.Size(63, 28);
            this.CSLabel.TabIndex = 13;
            this.CSLabel.Text = "0000";
            this.CSLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 182);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 16);
            this.label3.TabIndex = 12;
            this.label3.Text = "CS";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DLLabel
            // 
            this.DLLabel.BackColor = System.Drawing.SystemColors.Window;
            this.DLLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DLLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DLLabel.Location = new System.Drawing.Point(67, 137);
            this.DLLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DLLabel.Name = "DLLabel";
            this.DLLabel.Size = new System.Drawing.Size(27, 28);
            this.DLLabel.TabIndex = 11;
            this.DLLabel.Text = "00";
            this.DLLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DHLabel
            // 
            this.DHLabel.BackColor = System.Drawing.SystemColors.Window;
            this.DHLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DHLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DHLabel.Location = new System.Drawing.Point(31, 137);
            this.DHLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DHLabel.Name = "DHLabel";
            this.DHLabel.Size = new System.Drawing.Size(27, 28);
            this.DHLabel.TabIndex = 10;
            this.DHLabel.Text = "00";
            this.DHLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 143);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 16);
            this.label10.TabIndex = 9;
            this.label10.Text = "DX";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CLLabel
            // 
            this.CLLabel.BackColor = System.Drawing.SystemColors.Window;
            this.CLLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CLLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CLLabel.Location = new System.Drawing.Point(67, 97);
            this.CLLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CLLabel.Name = "CLLabel";
            this.CLLabel.Size = new System.Drawing.Size(27, 28);
            this.CLLabel.TabIndex = 8;
            this.CLLabel.Text = "00";
            this.CLLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CHLabel
            // 
            this.CHLabel.BackColor = System.Drawing.SystemColors.Window;
            this.CHLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.CHLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CHLabel.Location = new System.Drawing.Point(31, 97);
            this.CHLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CHLabel.Name = "CHLabel";
            this.CHLabel.Size = new System.Drawing.Size(27, 28);
            this.CHLabel.TabIndex = 7;
            this.CHLabel.Text = "00";
            this.CHLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 104);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "CX";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BLLabel
            // 
            this.BLLabel.BackColor = System.Drawing.SystemColors.Window;
            this.BLLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BLLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BLLabel.Location = new System.Drawing.Point(67, 58);
            this.BLLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BLLabel.Name = "BLLabel";
            this.BLLabel.Size = new System.Drawing.Size(27, 28);
            this.BLLabel.TabIndex = 5;
            this.BLLabel.Text = "00";
            this.BLLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BHLabel
            // 
            this.BHLabel.BackColor = System.Drawing.SystemColors.Window;
            this.BHLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.BHLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BHLabel.Location = new System.Drawing.Point(31, 58);
            this.BHLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BHLabel.Name = "BHLabel";
            this.BHLabel.Size = new System.Drawing.Size(27, 28);
            this.BHLabel.TabIndex = 4;
            this.BHLabel.Text = "00";
            this.BHLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 64);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "BX";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ALLabel
            // 
            this.ALLabel.BackColor = System.Drawing.SystemColors.Window;
            this.ALLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ALLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ALLabel.Location = new System.Drawing.Point(66, 20);
            this.ALLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ALLabel.Name = "ALLabel";
            this.ALLabel.Size = new System.Drawing.Size(27, 28);
            this.ALLabel.TabIndex = 2;
            this.ALLabel.Text = "00";
            this.ALLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AHLabel
            // 
            this.AHLabel.BackColor = System.Drawing.SystemColors.Window;
            this.AHLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AHLabel.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AHLabel.Location = new System.Drawing.Point(31, 19);
            this.AHLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AHLabel.Name = "AHLabel";
            this.AHLabel.Size = new System.Drawing.Size(27, 28);
            this.AHLabel.TabIndex = 1;
            this.AHLabel.Text = "00";
            this.AHLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "AX";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StackGroupBox
            // 
            this.StackGroupBox.Controls.Add(this.StackList);
            this.StackGroupBox.Location = new System.Drawing.Point(346, 13);
            this.StackGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.StackGroupBox.Name = "StackGroupBox";
            this.StackGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.StackGroupBox.Size = new System.Drawing.Size(172, 375);
            this.StackGroupBox.TabIndex = 2;
            this.StackGroupBox.TabStop = false;
            this.StackGroupBox.Text = "Stack";
            // 
            // StackList
            // 
            this.StackList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StackList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.StackList.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StackList.FormattingEnabled = true;
            this.StackList.Items.AddRange(new object[] {
            "0000:0001 FFFF",
            "0000:0002 EEEE",
            "0000:0003 DDDD",
            "0000:0004 CCCC",
            "0000:0005 BBBB",
            "0000:0006 AAAA"});
            this.StackList.Location = new System.Drawing.Point(4, 19);
            this.StackList.Name = "StackList";
            this.StackList.ScrollAlwaysVisible = true;
            this.StackList.Size = new System.Drawing.Size(164, 352);
            this.StackList.TabIndex = 0;
            // 
            // DasmList
            // 
            this.DasmList.FormattingEnabled = true;
            this.DasmList.Location = new System.Drawing.Point(525, 22);
            this.DasmList.Name = "DasmList";
            this.DasmList.Size = new System.Drawing.Size(433, 633);
            this.DasmList.TabIndex = 0;
            // 
            // MemoryTextBox
            // 
            this.MemoryTextBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MemoryTextBox.Location = new System.Drawing.Point(13, 426);
            this.MemoryTextBox.Multiline = true;
            this.MemoryTextBox.Name = "MemoryTextBox";
            this.MemoryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MemoryTextBox.Size = new System.Drawing.Size(501, 229);
            this.MemoryTextBox.TabIndex = 1;
            // 
            // RebootButton
            // 
            this.RebootButton.Location = new System.Drawing.Point(13, 662);
            this.RebootButton.Name = "RebootButton";
            this.RebootButton.Size = new System.Drawing.Size(86, 31);
            this.RebootButton.TabIndex = 5;
            this.RebootButton.Text = "Reboot";
            this.RebootButton.UseVisualStyleBackColor = true;
            this.RebootButton.Click += new System.EventHandler(this.RebootButton_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point(688, 661);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(86, 31);
            this.RefreshButton.TabIndex = 6;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(780, 661);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(86, 31);
            this.StopButton.TabIndex = 7;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(872, 662);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(86, 31);
            this.RunButton.TabIndex = 8;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // txtSegment
            // 
            this.txtSegment.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSegment.Location = new System.Drawing.Point(13, 400);
            this.txtSegment.Name = "txtSegment";
            this.txtSegment.Size = new System.Drawing.Size(44, 20);
            this.txtSegment.TabIndex = 9;
            this.txtSegment.Text = "0000";
            // 
            // txtOffset
            // 
            this.txtOffset.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOffset.Location = new System.Drawing.Point(63, 400);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.Size = new System.Drawing.Size(44, 20);
            this.txtOffset.TabIndex = 10;
            this.txtOffset.Text = "0000";
            // 
            // GoToButton
            // 
            this.GoToButton.Location = new System.Drawing.Point(113, 400);
            this.GoToButton.Name = "GoToButton";
            this.GoToButton.Size = new System.Drawing.Size(75, 23);
            this.GoToButton.TabIndex = 11;
            this.GoToButton.Text = "Go To";
            this.GoToButton.UseVisualStyleBackColor = true;
            this.GoToButton.Click += new System.EventHandler(this.GoToButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(971, 705);
            this.Controls.Add(this.GoToButton);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.txtSegment);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.RebootButton);
            this.Controls.Add(this.MemoryTextBox);
            this.Controls.Add(this.DasmList);
            this.Controls.Add(this.StackGroupBox);
            this.Controls.Add(this.RegistersGroupBox);
            this.Controls.Add(this.FlagsGroupBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CPU Monitor";
            this.FlagsGroupBox.ResumeLayout(false);
            this.RegistersGroupBox.ResumeLayout(false);
            this.RegistersGroupBox.PerformLayout();
            this.StackGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox FlagsGroupBox;
        private System.Windows.Forms.Label DFLabel;
        private System.Windows.Forms.Label IFLabel;
        private System.Windows.Forms.Label AFLabel;
        private System.Windows.Forms.Label PFLabel;
        private System.Windows.Forms.Label OFLabel;
        private System.Windows.Forms.Label SFLabel;
        private System.Windows.Forms.Label ZFLabel;
        private System.Windows.Forms.Label CFLabel;
        private System.Windows.Forms.GroupBox RegistersGroupBox;
        private System.Windows.Forms.GroupBox StackGroupBox;
        private System.Windows.Forms.Label TFLabel;
        private System.Windows.Forms.Label DILabel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label SILabel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label SPLabel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label BPLabel;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label ESLabel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label DSLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label SSLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label CSLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label DLLabel;
        private System.Windows.Forms.Label DHLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label CLLabel;
        private System.Windows.Forms.Label CHLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label BLLabel;
        private System.Windows.Forms.Label BHLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label ALLabel;
        private System.Windows.Forms.Label AHLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox StackList;
        private System.Windows.Forms.CheckedListBox DasmList;
        private System.Windows.Forms.TextBox MemoryTextBox;
        private System.Windows.Forms.Button RebootButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.TextBox txtSegment;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Button GoToButton;
    }
}

