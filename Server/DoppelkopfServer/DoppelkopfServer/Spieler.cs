using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DoppelkopfServer
{
    class Spieler
    {
        TcpClient Cl;
        Stream ClSt;
        public BinaryReader r;
        public BinaryWriter w;
        public String Name;
        public int Punktzahl;

        public Spieler(TcpClient Verb, String name)
        {
            Cl = Verb;
            ClSt = Cl.GetStream();
            r = new BinaryReader(ClSt);
            w = new BinaryWriter(ClSt);
            this.Name = name;
        }

        public void WaitForInfo()
        {
            String Nachricht = r.ReadString();
        }

        public int WaitForCard()
        {
            int ID =(int) r.ReadInt64();
            return ID;
        }

        public void SendText(String nachricht)
        {
            w.Write(nachricht);
        }

        public void SendNumber(Int64 ID)
        {
            w.Write(ID);
        }

        public void StichPunkte(int Punkte)
        {
            Punktzahl += Punkte;
        }
    }
}
