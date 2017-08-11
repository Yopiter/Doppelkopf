namespace DoppelkopfClient
{
    partial class Modus_Auswahl
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
            this.BT_Bereit = new System.Windows.Forms.Button();
            this.Modus_aktuell = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BT_Bereit
            // 
            this.BT_Bereit.Location = new System.Drawing.Point(103, 227);
            this.BT_Bereit.Name = "BT_Bereit";
            this.BT_Bereit.Size = new System.Drawing.Size(75, 23);
            this.BT_Bereit.TabIndex = 0;
            this.BT_Bereit.Text = "Bereit";
            this.BT_Bereit.UseVisualStyleBackColor = true;
            this.BT_Bereit.Click += new System.EventHandler(this.BT_Bereit_Click);
            // 
            // Modus_aktuell
            // 
            this.Modus_aktuell.AutoSize = true;
            this.Modus_aktuell.Location = new System.Drawing.Point(12, 9);
            this.Modus_aktuell.Name = "Modus_aktuell";
            this.Modus_aktuell.Size = new System.Drawing.Size(76, 13);
            this.Modus_aktuell.TabIndex = 1;
            this.Modus_aktuell.Text = "Modus_aktuell";
            // 
            // Modus_Auswahl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.Modus_aktuell);
            this.Controls.Add(this.BT_Bereit);
            this.Name = "Modus_Auswahl";
            this.Text = "Modus_Auswahl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Bereit;
        private System.Windows.Forms.Label Modus_aktuell;
    }
}