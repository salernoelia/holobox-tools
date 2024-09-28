namespace holobox_tools
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtOSCAddress = new System.Windows.Forms.TextBox();
            this.txtOSCPort = new System.Windows.Forms.TextBox();
            this.cmbWebcam = new System.Windows.Forms.ComboBox();
            this.cmbMicrophone = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label(); // Beispiel-Label
            this.SuspendLayout();
            // 
            // txtOSCAddress
            // 
            this.txtOSCAddress.Location = new System.Drawing.Point(100, 20);
            this.txtOSCAddress.Name = "txtOSCAddress";
            this.txtOSCAddress.Size = new System.Drawing.Size(150, 20);
            this.txtOSCAddress.TabIndex = 0;
            // 
            // txtOSCPort
            // 
            this.txtOSCPort.Location = new System.Drawing.Point(100, 60);
            this.txtOSCPort.Name = "txtOSCPort";
            this.txtOSCPort.Size = new System.Drawing.Size(150, 20);
            this.txtOSCPort.TabIndex = 1;
            // 
            // cmbWebcam
            // 
            this.cmbWebcam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWebcam.FormattingEnabled = true;
            this.cmbWebcam.Location = new System.Drawing.Point(100, 100);
            this.cmbWebcam.Name = "cmbWebcam";
            this.cmbWebcam.Size = new System.Drawing.Size(150, 21);
            this.cmbWebcam.TabIndex = 2;
            // 
            // cmbMicrophone
            // 
            this.cmbMicrophone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMicrophone.FormattingEnabled = true;
            this.cmbMicrophone.Location = new System.Drawing.Point(100, 140);
            this.cmbMicrophone.Name = "cmbMicrophone";
            this.cmbMicrophone.Size = new System.Drawing.Size(150, 21);
            this.cmbMicrophone.TabIndex = 3;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(100, 180);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(70, 30);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(180, 180);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(70, 30);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lstLog
            // 
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(300, 20);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(400, 290);
            this.lstLog.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "OSC Adresse:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(734, 341);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.cmbMicrophone);
            this.Controls.Add(this.cmbWebcam);
            this.Controls.Add(this.txtOSCPort);
            this.Controls.Add(this.txtOSCAddress);
            this.Name = "Form1";
            this.Text = "Holobox Tools";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOSCAddress;
        private System.Windows.Forms.TextBox txtOSCPort;
        private System.Windows.Forms.ComboBox cmbWebcam;
        private System.Windows.Forms.ComboBox cmbMicrophone;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.Label label1;
    }
}
