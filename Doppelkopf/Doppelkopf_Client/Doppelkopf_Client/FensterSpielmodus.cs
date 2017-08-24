using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Doppelkopf_Client
{
    public partial class FensterSpielmodus : Form
    {
        List<Karte> Hand;
        int AnzahlAlte = 0;
        int AnzahlKönige = 0;
        int AnzahlTrumpf = 0;
        public Spielmodus ChosenMode;
        public bool Re;
        public bool Kontra;

        public FensterSpielmodus(List<Karte> Handkarten)
        {
            InitializeComponent();
            Hand = Handkarten;
            HandAnalysieren();
            SchaltflaechenVerstecken();
        }

        private void HandAnalysieren()
        {
            foreach (Karte K in Hand)
            {
                if (K.IstAlte()) AnzahlAlte++;
                if (K.GetWert() == Kartenwert.König) AnzahlKönige++;
                if (K.trumpfstärke > -1) AnzahlTrumpf++;
            }
        }

        private void SchaltflaechenVerstecken()
        {
            if (AnzahlAlte > 1) cbKontra.Enabled = false;
            else cbRe.Enabled = false;
            btHochzeit.Enabled = (AnzahlAlte == 2);
            btNormal.Text = AnzahlAlte == 2 ? "Stilles Solo" : "Normales Spiel";
        }

        private void BtHochzeit_Click(object sender, EventArgs e)
        {
            ChosenMode = Spielmodus.Hochzeit;
            Kontra = false;
            Re = cbRe.Checked;
            Hide();
        }

        private void BtNormal_Click(object sender, EventArgs e)
        {
            ChosenMode = AnzahlAlte == 2 ? Spielmodus.StillesSolo : Spielmodus.Normal;
            Re = cbRe.Checked;
            Kontra = cbKontra.Checked;
            Hide();
        }
    }
}
