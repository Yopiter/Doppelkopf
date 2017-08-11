namespace Doppelkopf_Client
{
    partial class FensterSpielmodus
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
            this.cbRe = new System.Windows.Forms.CheckBox();
            this.cbKontra = new System.Windows.Forms.CheckBox();
            this.btNormal = new System.Windows.Forms.Button();
            this.btHochzeit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbRe
            // 
            this.cbRe.AutoSize = true;
            this.cbRe.Location = new System.Drawing.Point(12, 36);
            this.cbRe.Name = "cbRe";
            this.cbRe.Size = new System.Drawing.Size(40, 17);
            this.cbRe.TabIndex = 0;
            this.cbRe.Text = "Re";
            this.cbRe.UseVisualStyleBackColor = true;
            // 
            // cbKontra
            // 
            this.cbKontra.AutoSize = true;
            this.cbKontra.Location = new System.Drawing.Point(12, 59);
            this.cbKontra.Name = "cbKontra";
            this.cbKontra.Size = new System.Drawing.Size(57, 17);
            this.cbKontra.TabIndex = 1;
            this.cbKontra.Text = "Kontra";
            this.cbKontra.UseVisualStyleBackColor = true;
            // 
            // btNormal
            // 
            this.btNormal.Location = new System.Drawing.Point(98, 32);
            this.btNormal.Name = "btNormal";
            this.btNormal.Size = new System.Drawing.Size(103, 23);
            this.btNormal.TabIndex = 2;
            this.btNormal.Text = "Normales Spiel";
            this.btNormal.UseVisualStyleBackColor = true;
            this.btNormal.Click += new System.EventHandler(this.BtNormal_Click);
            // 
            // btHochzeit
            // 
            this.btHochzeit.Location = new System.Drawing.Point(98, 55);
            this.btHochzeit.Name = "btHochzeit";
            this.btHochzeit.Size = new System.Drawing.Size(103, 23);
            this.btHochzeit.TabIndex = 3;
            this.btHochzeit.Text = "Hochzeit";
            this.btHochzeit.UseVisualStyleBackColor = true;
            this.btHochzeit.Click += new System.EventHandler(this.BtHochzeit_Click);
            // 
            // Spielmodus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btHochzeit);
            this.Controls.Add(this.btNormal);
            this.Controls.Add(this.cbKontra);
            this.Controls.Add(this.cbRe);
            this.Name = "Spielmodus";
            this.Text = "Spielmodus wählen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbRe;
        private System.Windows.Forms.CheckBox cbKontra;
        private System.Windows.Forms.Button btNormal;
        private System.Windows.Forms.Button btHochzeit;
    }
}