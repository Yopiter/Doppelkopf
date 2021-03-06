﻿using System;

namespace Doppelkopf_Server
{
    public class Karte //DLL
    {
        public int id;
        public int trumpfstärke; //-1 -> kein Trumpf
        public int punktzahl;
        public Farben farbe;
        Kartenwert kartenWert;

        string kartenName;

        public Karte(int KFarbe, int KWert, int edition)
        {
            id = 12 * KFarbe + 2 * KWert + edition;

            farbe = (Farben)KFarbe;
            kartenWert = (Kartenwert)KWert;

            //TODO spezielle Kartennamen einbauen
            kartenName = Enum.GetName(typeof(Farben), farbe) + " " + Enum.GetName(typeof(Kartenwert), kartenWert);

            trumpfstärke = StärkeBerechnen(KFarbe, KWert);
            punktzahl = PunkzahlBestimmung();
        }

        private int StärkeBerechnen(int farbWert, int wertigkeit)
        {
            if (!(farbWert == 0 || id == 20 || id == 21 || wertigkeit == 3 || wertigkeit == 2)) return -1; //20 & 21 == Herz Zehn, höchster Trumpf im Spiel
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

        public Kartenwert GetWert()
        {
            return kartenWert;
        }
    }
}