using System;

namespace Doppelkopf_Client
{
    public class Karte
    {
        public int ID;
        public int Trumpfstärke; //-1 -> kein Trumpf
        public int Punktzahl;
        Farben Farbe;
        Kartenwert kWert;

        string Wert;
        string Name;

        public Karte(int KFarbe, int KWert, string KName, int edition)
        {
            ID = 12 * KFarbe + 2 * KWert + edition;

            Farbe = (Farben) KFarbe;
            kWert = (Kartenwert) KWert;
            
            Name = KName;
            if (Name == "")
                Name = Enum.GetName(typeof(Farben), Farbe) + " " + Enum.GetName(typeof(Kartenwert), kWert);

            Trumpfstärke = StärkeBerechnen(KFarbe, KWert);
            Punktzahl = PunkzahlBestimmung();
        }

        private int StärkeBerechnen(int farbWert, int wertigkeit)
        {
            if (!(farbWert == 0 || ID == 14 || ID == 15 || wertigkeit == 3 || wertigkeit == 2)) return -1;
            if (farbWert == 0 && wertigkeit != 2 && wertigkeit != 3)
            {
                return wertigkeit;
            }
            return 10 * wertigkeit + farbWert;
        }

        private int PunkzahlBestimmung()
        {
            int[] PunkteArray = new int[] { 0, 4, 2, 3, 10, 11 };
            return PunkteArray[(int)kWert];
        }

        public bool IstAlte()
        {
            return Farbe == Farben.Eichel && kWert == Kartenwert.Ober;
        }

        public string GetWert()
        {
            return Wert;
        }
    }
}
