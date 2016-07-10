using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoppelkopfClient
{
    class Karte
    {
        public int ID;
        public int Farbwert;
        public int Wertzahl;
        public int Trumpfstärke;
        public int Punktzahl;
        String Farbe;
        String Wert;
        public String Name;
        int Edition;

        public Karte(int KFarbe, int KWert, int edition)
        {
            Farbwert = KFarbe;
            Wertzahl = KWert;
            Edition = edition;
            ID = 12 * Farbwert + 2 * Wertzahl + Edition;
            if (Farbwert == 0) Farbe = "Schellen";
            if (Farbwert == 1) Farbe = "Herz";
            if (Farbwert == 2) Farbe = "Blatt";
            if (Farbwert == 3) Farbe = "Eichel";

            if (Wertzahl == 0) Wert = "9";
            if (Wertzahl == 1) Wert = "König";
            if (Wertzahl == 2) Wert = "Unter";
            if (Wertzahl == 3) Wert = "Ober";
            if (Wertzahl == 4) Wert = "10";
            if (Wertzahl == 5) Wert = "Ass";

            if (ID == 20 || ID == 21) Name = "Dulle";
            if (ID == 42 || ID == 43) Name = "Alte";
            if (ID == 10 || ID == 11) Name = "Fuchs";
            if (Name == "") Name = Farbe + " " + Wert;

            Trumpfstärke = Stärkeberechnen();

            Punktzahl = PunkzahlBestimmung();
        }

        private int Stärkeberechnen()
        {
            if (!(Farbwert == 0 || ID == 20 || ID == 21 || Wertzahl == 3 || Wertzahl == 2)) return -1;
            if (Farbwert == 0 && Wertzahl != 2 && Wertzahl != 3)
            {
                return Wertzahl;
            }
            return 10 * Wertzahl + Farbwert;
        }

        private int PunkzahlBestimmung()
        {
            int[] PunkteArray = new int[] { 0, 4, 2, 3, 10, 11 };
            return PunkteArray[Wertzahl];
        }
    }
}
