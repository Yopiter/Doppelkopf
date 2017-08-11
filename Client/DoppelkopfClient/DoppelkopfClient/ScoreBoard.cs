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
    public partial class ScoreBoard : Form
    {
        public ScoreBoard(String[] ReNamenPunkteAch, String[] Kontra, String SpielBeschr)
        {
            InitializeComponent();
            Label_Spiel.Text = SpielBeschr;

            Re_1.Text = ReNamenPunkteAch[0];
            Re_2.Text = ReNamenPunkteAch[1];
            Punkte_Re1.Text = ReNamenPunkteAch[2];
            Punkte_Re2.Text = ReNamenPunkteAch[3];
            Re_Ges.Text = ReNamenPunkteAch[4];
            Re_Ach.Text = ReNamenPunkteAch[5];

            Kontra_1.Text = Kontra[0];
            Kontra_2.Text = Kontra[1];
            Punkte_Kontra1.Text = Kontra[2];
            Punkte_Kontra2.Text = Kontra[3];
            Kontra_Ges.Text = Kontra[4];
            Kontra_Ach.Text = Kontra[5];
        }
    }
}
