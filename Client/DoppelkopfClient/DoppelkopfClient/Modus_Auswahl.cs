using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoppelkopfClient
{
    public partial class Modus_Auswahl : Form
    {
        public String NeuerModus;
        public Modus_Auswahl(String ModusAlt)
        {
            InitializeComponent();
            Modus_aktuell.Text = ModusAlt;
            NeuerModus = ModusAlt;
        }

        private void BT_Bereit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
