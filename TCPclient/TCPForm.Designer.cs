namespace TCPclient
{
    partial class TCPForm
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
            this.bClientStart = new System.Windows.Forms.Button();
            this.clientIn = new System.Windows.Forms.TextBox();
            this.clientIP = new System.Windows.Forms.TextBox();
            this.clientPort = new System.Windows.Forms.NumericUpDown();
            this.textOut = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.clientPort)).BeginInit();
            this.SuspendLayout();
            // 
            // bClientStart
            // 
            this.bClientStart.Location = new System.Drawing.Point(670, 28);
            this.bClientStart.Name = "bClientStart";
            this.bClientStart.Size = new System.Drawing.Size(75, 23);
            this.bClientStart.TabIndex = 0;
            this.bClientStart.Text = "Connect to Server";
            this.bClientStart.UseVisualStyleBackColor = true;
            this.bClientStart.Click += new System.EventHandler(this.bClientStart_Click);
            // 
            // clientIn
            // 
            this.clientIn.Location = new System.Drawing.Point(23, 28);
            this.clientIn.Multiline = true;
            this.clientIn.Name = "clientIn";
            this.clientIn.Size = new System.Drawing.Size(566, 139);
            this.clientIn.TabIndex = 1;
            // 
            // clientIP
            // 
            this.clientIP.Location = new System.Drawing.Point(639, 92);
            this.clientIP.Name = "clientIP";
            this.clientIP.Size = new System.Drawing.Size(100, 20);
            this.clientIP.TabIndex = 2;
            this.clientIP.Text = "192.168.0.5";
            // 
            // clientPort
            // 
            this.clientPort.Location = new System.Drawing.Point(639, 147);
            this.clientPort.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.clientPort.Name = "clientPort";
            this.clientPort.Size = new System.Drawing.Size(120, 20);
            this.clientPort.TabIndex = 3;
            this.clientPort.Value = new decimal(new int[] {
            5004,
            0,
            0,
            0});
            // 
            // textOut
            // 
            this.textOut.Location = new System.Drawing.Point(29, 210);
            this.textOut.Name = "textOut";
            this.textOut.Size = new System.Drawing.Size(559, 20);
            this.textOut.TabIndex = 4;
            this.textOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextOut_KeyUp);
            // 
            // TCPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 313);
            this.Controls.Add(this.textOut);
            this.Controls.Add(this.clientPort);
            this.Controls.Add(this.clientIP);
            this.Controls.Add(this.clientIn);
            this.Controls.Add(this.bClientStart);
            this.Name = "TCPForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.clientPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bClientStart;
        private System.Windows.Forms.TextBox clientIn;
        private System.Windows.Forms.TextBox clientIP;
        private System.Windows.Forms.NumericUpDown clientPort;
        private System.Windows.Forms.TextBox textOut;
    }
}

