namespace Doppelkopf_Client
{
    partial class LogIn
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
            this.TB_Name = new System.Windows.Forms.TextBox();
            this.TB_IP = new System.Windows.Forms.TextBox();
            this.NB_Port = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LB_Status = new System.Windows.Forms.Label();
            this.BT_Verb = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NB_Port)).BeginInit();
            this.SuspendLayout();
            // 
            // TB_Name
            // 
            this.TB_Name.Location = new System.Drawing.Point(18, 21);
            this.TB_Name.Name = "TB_Name";
            this.TB_Name.Size = new System.Drawing.Size(100, 20);
            this.TB_Name.TabIndex = 0;
            this.TB_Name.Text = "GenericName";
            this.TB_Name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TB_Name_KeyDown);
            // 
            // TB_IP
            // 
            this.TB_IP.Location = new System.Drawing.Point(18, 47);
            this.TB_IP.Name = "TB_IP";
            this.TB_IP.Size = new System.Drawing.Size(100, 20);
            this.TB_IP.TabIndex = 1;
            this.TB_IP.Text = "127.0.0.1";
            // 
            // NB_Port
            // 
            this.NB_Port.Location = new System.Drawing.Point(18, 73);
            this.NB_Port.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.NB_Port.Name = "NB_Port";
            this.NB_Port.Size = new System.Drawing.Size(100, 20);
            this.NB_Port.TabIndex = 2;
            this.NB_Port.Value = new decimal(new int[] {
            666,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(136, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Host-IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(136, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port";
            // 
            // LB_Status
            // 
            this.LB_Status.AutoSize = true;
            this.LB_Status.Location = new System.Drawing.Point(15, 106);
            this.LB_Status.Name = "LB_Status";
            this.LB_Status.Size = new System.Drawing.Size(81, 13);
            this.LB_Status.TabIndex = 6;
            this.LB_Status.Text = "Keine Probleme";
            // 
            // BT_Verb
            // 
            this.BT_Verb.Location = new System.Drawing.Point(18, 137);
            this.BT_Verb.Name = "BT_Verb";
            this.BT_Verb.Size = new System.Drawing.Size(160, 23);
            this.BT_Verb.TabIndex = 7;
            this.BT_Verb.Text = "Verbinden";
            this.BT_Verb.UseVisualStyleBackColor = true;
            this.BT_Verb.Click += new System.EventHandler(this.BT_Verb_Click);
            // 
            // LogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 183);
            this.Controls.Add(this.BT_Verb);
            this.Controls.Add(this.LB_Status);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NB_Port);
            this.Controls.Add(this.TB_IP);
            this.Controls.Add(this.TB_Name);
            this.Name = "LogIn";
            this.Text = "Verbindung herstellen";
            ((System.ComponentModel.ISupportInitialize)(this.NB_Port)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Name;
        private System.Windows.Forms.TextBox TB_IP;
        private System.Windows.Forms.NumericUpDown NB_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label LB_Status;
        private System.Windows.Forms.Button BT_Verb;
    }
}

