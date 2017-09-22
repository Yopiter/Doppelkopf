using System;

namespace Doppelkopf_Client
{
    public class Karte
    {
        public int id;
        public int trumpfstärke; //-1 -> kein Trumpf
        public int punktzahl;
        public Farben farbe;
        public Kartenwert kartenWert;

        string kartenName;
        static int[] PunkteArray = new int[] { 0, 4, 2, 3, 10, 11 };

        public Karte(int KFarbe, int KWert, int edition)
        {
            id = 12 * KFarbe + 2 * KWert + edition;

            farbe = (Farben)KFarbe;
            kartenWert = (Kartenwert)KWert;

            switch (id)
            {
                case (20):
                case (21):
                    kartenName = "Dulle";
                    break;
                case (42):
                case (43):
                    kartenName = "Alte";
                    break;
                case (40):
                case (41):
                    kartenName = "Karlchen";
                    break;
                case (10):
                case (11):
                    kartenName = "Fuchs";
                    break;
                default:
                    kartenName = Enum.GetName(typeof(Farben), farbe) + " " + Enum.GetName(typeof(Kartenwert), kartenWert);
                    break;
            }

            trumpfstärke = StärkeBerechnen(KFarbe, KWert);
            punktzahl = PunkzahlBestimmung();
        }

        private int StärkeBerechnen(int farbWert, int wertigkeit)
        {
            if (!(farbWert == 0 || id == 20 || id == 21 || wertigkeit == 3 || wertigkeit == 2)) return -1;
            if (farbWert == 0 && wertigkeit != 2 && wertigkeit != 3)
            {
                return wertigkeit;
            }
            return 10 * wertigkeit + farbWert;
        }

        private int PunkzahlBestimmung()
        {
            return PunkteArray[(int)kartenWert];
        }

        public bool IstAlte()
        {
            return farbe == Farben.Eichel && kartenWert == Kartenwert.Ober;
        }

        public string GetImagePath()
        {
            return string.Format("Ress\\Karten_Template\\{0}\\{1}.png", (int)farbe, (int)kartenWert);
        }

        internal bool IsValid(Farben stichfarbe, bool Trumped)
        {
            bool ThisIsATrumpf = trumpfstärke < 0;
            return (Trumped && ThisIsATrumpf) || (stichfarbe == farbe && Trumped == ThisIsATrumpf);
        }
    }
}
