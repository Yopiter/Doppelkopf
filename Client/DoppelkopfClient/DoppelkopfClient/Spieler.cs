using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DoppelkopfClient
{
    class Spieler
    {
        public String Name;
        public int Punkte;
        public bool Alte;
        public int Position;
        public int KartenAnzahl;
        Button KarteBox;
        Label Name_Label;

        public Spieler(String name,int position)
        {
            Name = name;
            Position = position;
            Alte = false;
            Punkte = 0;
            KartenAnzahl = 12;
        }

        public void BoxZuweisen(Button PB)
        {
            KarteBox = PB;
        }

        public void KarteGelegt(Karte Card)
        {
            KarteBox.Image=new Bitmap("Ress\\Karten_Template\\"+Card.Farbwert.ToString()+"\\"+Card.Wertzahl.ToString()+".png");
            KarteBox.Visible = true;
            KartenAnzahl--;
        }
    }
}
