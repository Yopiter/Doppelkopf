using System;
using System.Collections.Generic;

namespace Doppelkopf_Server
{
    class Spiel
    {
        public Spielmodus gameMode;
        public List<Spieler> TeamRe;
        public List<Spieler> TeamKontra;
        //TODO hier die Stiche Reinpacken + Ablauf?

        public enum Spielmodus { Normal, Hochzeit, StillesSolo };

        public Spiel()
        {
            gameMode = Spielmodus.Normal; //Default
        }
    }
}
