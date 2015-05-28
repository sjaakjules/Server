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
            this.udpIP = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPort)).BeginInit();
            this.SuspendLayout();
            // 
            // serverIn
            // 
            this.serverIn.Location = new System.Drawing.Point(33, 43);
            this.serverIn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.serverIn.Multiline = true;
            this.serverIn.Name = "serverIn";
            this.serverIn.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.serverIn.Size = new System.Drawing.Size(1090, 338);
            this.serverIn.TabIndex = 0;
            // 
            // tcpPort
            // 
            this.tcpPort.Location = new System.Drawing.Point(1209, 422);
            this.tcpPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tcpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.tcpPort.Name = "tcpPort";
            this.tcpPort.Size = new System.Drawing.Size(87, 26);
            this.tcpPort.TabIndex = 2;
            this.tcpPort.Value = new decimal(new int[] {
            5004,
            0,
            0,
            0});
            // 
            // udpPort
            // 
            this.udpPort.Location = new System.Drawing.Point(1209, 163);
            this.udpPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.udpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.udpPort.Name = "udpPort";
            this.udpPort.Size = new System.Drawing.Size(180, 26);
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
            this.label1.Location = new System.Drawing.Point(1204, 371);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "TCP Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1204, 112);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "UDP Settings";
            // 
            // serverOut
            // 
            this.serverOut.Location = new System.Drawing.Point(36, 420);
            this.serverOut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.serverOut.Multiline = true;
            this.serverOut.Name = "serverOut";
            this.serverOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.serverOut.Size = new System.Drawing.Size(1086, 299);
            this.serverOut.TabIndex = 6;
            // 
            // bUDP
            // 
            this.bUDP.Location = new System.Drawing.Point(1209, 43);
            this.bUDP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bUDP.Name = "bUDP";
            this.bUDP.Size = new System.Drawing.Size(180, 35);
            this.bUDP.TabIndex = 7;
            this.bUDP.Text = "Start UDP server";
            this.bUDP.UseVisualStyleBackColor = true;
            this.bUDP.Click += new System.EventHandler(this.bUDP_Click);
            // 
            // bTCP
            // 
            this.bTCP.Location = new System.Drawing.Point(1209, 294);
            this.bTCP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bTCP.Name = "bTCP";
            this.bTCP.Size = new System.Drawing.Size(180, 35);
            this.bTCP.TabIndex = 8;
            this.bTCP.Text = "Start TCP server";
            this.bTCP.UseVisualStyleBackColor = true;
            this.bTCP.Click += new System.EventHandler(this.bTCP_Click);
            // 
            // udpIP
            // 
            this.udpIP.Location = new System.Drawing.Point(1210, 219);
            this.udpIP.Name = "udpIP";
            this.udpIP.Size = new System.Drawing.Size(179, 26);
            this.udpIP.TabIndex = 9;
            this.udpIP.Text = "192.168.2.19";
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1446, 765);
            this.Controls.Add(this.udpIP);
            this.Controls.Add(this.bTCP);
            this.Controls.Add(this.bUDP);
            this.Controls.Add(this.serverOut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udpPort);
            this.Controls.Add(this.tcpPort);
            this.Controls.Add(this.serverIn);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
        private System.Windows.Forms.TextBox udpIP;
    }
}

