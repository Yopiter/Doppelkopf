using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoppelkopfClient
{
    class Team
    {
        public String Name;
        public int ID;
        public List<Spieler> SpielerList;

        public Team(String name, int id)
        {
            Name = name;
            ID = id;
            SpielerList = new List<Spieler>();
        }

        public void AddSpieler(Spieler Playa)
        {
            SpielerList.Add(Playa);
        }

        public int GesamtPunkte()
        {
            return SpielerList[0].Punkte + SpielerList[1].Punkte;
        }
    }
}
