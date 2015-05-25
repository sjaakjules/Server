namespace Server
{
    partial class ServerForm
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
            this.serverIn = new System.Windows.Forms.TextBox();
            this.tcpPort = new System.Windows.Forms.NumericUpDown();
            this.udpPort = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serverOut = new System.Windows.Forms.TextBox();
            this.bUDP = new System.Windows.Forms.Button();
            this.bTCP = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // serverIn
            // 
            this.serverIn.Location = new System.Drawing.Point(22, 28);
            this.serverIn.Multiline = true;
            this.serverIn.Name = "serverIn";
            this.serverIn.Size = new System.Drawing.Size(728, 221);
            this.serverIn.TabIndex = 0;
            // 
            // tcpPort
            // 
            this.tcpPort.Location = new System.Drawing.Point(806, 274);
            this.tcpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.tcpPort.Name = "tcpPort";
            this.tcpPort.Size = new System.Drawing.Size(58, 20);
            this.tcpPort.TabIndex = 2;
            this.tcpPort.Value = new decimal(new int[] {
            5004,
            0,
            0,
            0});
            // 
            // udpPort
            // 
            this.udpPort.Location = new System.Drawing.Point(806, 106);
            this.udpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.udpPort.Name = "udpPort";
            this.udpPort.Size = new System.Drawing.Size(120, 20);
            this.udpPort.TabIndex = 3;
            this.udpPort.Value = new decimal(new int[] {
            5004,
            0,
            0,
            0});
            this.udpPort.ValueChanged += new System.EventHandler(this.udpPort_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(803, 241);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "TCP Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(803, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "UDP Settings";
            // 
            // serverOut
            // 
            this.serverOut.Location = new System.Drawing.Point(24, 273);
            this.serverOut.Multiline = true;
            this.serverOut.Name = "serverOut";
            this.serverOut.Size = new System.Drawing.Size(725, 196);
            this.serverOut.TabIndex = 6;
            // 
            // bUDP
            // 
            this.bUDP.Location = new System.Drawing.Point(806, 28);
            this.bUDP.Name = "bUDP";
            this.bUDP.Size = new System.Drawing.Size(120, 23);
            this.bUDP.TabIndex = 7;
            this.bUDP.Text = "Start UDP server";
            this.bUDP.UseVisualStyleBackColor = true;
            this.bUDP.Click += new System.EventHandler(this.bUDP_Click);
            // 
            // bTCP
            // 
            this.bTCP.Location = new System.Drawing.Point(806, 191);
            this.bTCP.Name = "bTCP";
            this.bTCP.Size = new System.Drawing.Size(120, 23);
            this.bTCP.TabIndex = 8;
            this.bTCP.Text = "Start TCP server";
            this.bTCP.UseVisualStyleBackColor = true;
            this.bTCP.Click += new System.EventHandler(this.bTCP_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 497);
            this.Controls.Add(this.bTCP);
            this.Controls.Add(this.bUDP);
            this.Controls.Add(this.serverOut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udpPort);
            this.Controls.Add(this.tcpPort);
            this.Controls.Add(this.serverIn);
            this.Name = "ServerForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.tcpPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox serverIn;
        private System.Windows.Forms.NumericUpDown tcpPort;
        private System.Windows.Forms.NumericUpDown udpPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverOut;
        private System.Windows.Forms.Button bUDP;
        private System.Windows.Forms.Button bTCP;
    }
}

