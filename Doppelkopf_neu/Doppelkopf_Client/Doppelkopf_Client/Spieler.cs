using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doppelkopf_Client
{
    class Spieler
    {
        public string Name;
        public int Punkte;
        public List<Zustand> ZustandsList;
        public int Kartenzahl;
        public Button btKarte;
        public TextBox tbLabel;
        public int AbsolutePositionAufServer;
        List<PictureBox> Zustandsboxen = new List<PictureBox>();

        public Spieler(string name, Button button, TextBox labelbox, int Position)
        {
            Name = name;
            Punkte = 0;
            ZustandsList = new List<Zustand>();
            btKarte = button;
            tbLabel = labelbox;
        }

        public void AddZustand(Zustand NeuerZustand, PictureBox newBox)
        {
            switch (NeuerZustand)
            {
                case (Zustand.Re):
                    newBox.Image = new Bitmap("Ress\\Icons\\Re.png");
                    break;
                case (Zustand.Kontra):
                    newBox.Image = new Bitmap("Ress\\Icons\\Re.png");
                    break;
                default:
                    MessageBox.Show("Oh no, Your Player " + Name + " seems to be retarted. He thinks he can be " + NeuerZustand.ToString());
                    break;
            }
            Zustandsboxen.Add(newBox);
            ZustandsList.Add(NeuerZustand);

        }
    }
}
