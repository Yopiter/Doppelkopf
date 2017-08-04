using System;
using System.Collections.Generic;
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

        public enum Zustand { Re, HochzeitAlte, HochzeitOhneAlte, Solo, Kontra };

        public Spieler(string name, Button button, TextBox labelbox, int Position)
        {
            Name = name;
            Punkte = 0;
            ZustandsList = new List<Zustand>();
            btKarte = button;
            tbLabel = labelbox;
        }

        public void AddZustand(Zustand NeuerZustand)
        {
            ZustandsList.Add(NeuerZustand);
        }
    }
}
