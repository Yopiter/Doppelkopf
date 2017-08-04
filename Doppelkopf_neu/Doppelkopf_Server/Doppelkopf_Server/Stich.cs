﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doppelkopf_Server
{
    class Stich
    {
        Spieler StartSpieler;
        public Spieler SpielerGingAn;
        public int StichPunktwert;
        bool gestochen;
        int StichFarbe;    //0=Shell = Trumpf
        List<Karte> KList;
        List<Spieler> SpielerList;

        public Stich(List<Spieler> SpielerReihenfolge)
        {
            SpielerList = SpielerReihenfolge;
            StartSpieler = SpielerReihenfolge[0];
        }

        public void StartkarteDefinieren(Karte StK)
        {
            KList = new List<Karte>() { StK };
            if (StK.Trumpfstärke == -1) StichFarbe = StK.Farbwert;
            else StichFarbe = 0;     //farbwert=0 bedeutet Trumpfstich
            gestochen = false;
        }

        public void KarteGespielt(Karte GelegteKarte)
        {
            KList.Add(GelegteKarte);
            if (KList.Count == 1) StartkarteDefinieren(GelegteKarte);
            if (KList.Count == 4) SiegerBestimmen();
        }

        private void SiegerBestimmen()
        {
            foreach (Karte k in KList)
            {
                if (k.Trumpfstärke != -1)
                {
                    Trumpfstich();
                    WertBestimmen();
                    return;
                }
            }
            Karte HCard = KList[0];
            foreach (Karte k in KList)
            {
                if (k.Farbwert == StichFarbe && k.Wertzahl > HCard.Wertzahl)
                {
                    HCard = k;
                }
            }
            SpielerGingAn = SpielerList[KList.IndexOf(HCard)];
            gestochen = false;
            WertBestimmen();
        }

        private void Trumpfstich()
        {
            Karte HCard = KList[0];
            foreach (Karte k in KList)
            {
                if (k.Trumpfstärke > HCard.Trumpfstärke)
                {
                    HCard = k;
                }
            }
            SpielerGingAn = SpielerList[KList.IndexOf(HCard)];
            gestochen = true;
        }

        private void WertBestimmen()
        {
            StichPunktwert = 0;
            foreach (Karte k in KList)
            {
                StichPunktwert += k.Punktzahl;
            }
            SpielerGingAn.AddPunkteVonStich(StichPunktwert);
        }

        public bool IsFarbstich()
        {
            return StichFarbe != 0;
        }

        public bool IsGestochen()
        {
            return gestochen;
        }
    }
}
