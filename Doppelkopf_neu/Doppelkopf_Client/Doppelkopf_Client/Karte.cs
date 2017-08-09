using System;

namespace Doppelkopf_Client
{
    public class Karte
    {
        public int id;
        public int trumpfstärke; //-1 -> kein Trumpf
        public int punktzahl;
        Farben farbe;
        Kartenwert kartenWert;

        string kartenName;

        public Karte(int KFarbe, int KWert, int edition)
        {
            id = 12 * KFarbe + 2 * KWert + edition;

            farbe = (Farben) KFarbe;
            kartenWert = (Kartenwert) KWert;
            
            //TODO spezielle Kartennamen einbauen
            kartenName = Enum.GetName(typeof(Farben), farbe) + " " + Enum.GetName(typeof(Kartenwert), kartenWert);

            trumpfstärke = StärkeBerechnen(KFarbe, KWert);
            punktzahl = PunkzahlBestimmung();
        }

        private int StärkeBerechnen(int farbWert, int wertigkeit)
        {
            if (!(farbWert == 0 || id == 14 || id == 15 || wertigkeit == 3 || wertigkeit == 2)) return -1;
            if (farbWert == 0 && wertigkeit != 2 && wertigkeit != 3)
            {
                return wertigkeit;
            }
            return 10 * wertigkeit + farbWert;
        }

        private int PunkzahlBestimmung()
        {
            int[] PunkteArray = new int[] { 0, 4, 2, 3, 10, 11 };
            return PunkteArray[(int)kartenWert];
        }

        public bool IstAlte()
        {
            return farbe == Farben.Eichel && kartenWert == Kartenwert.Ober;
        }
    }
}
