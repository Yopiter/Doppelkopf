using System;
using System.IO;
using System.Net.Sockets;

namespace Doppelkopf_Server
{
    class Spieler
    {
        TcpClient Cl;
        Stream ClSt;
        private BinaryReader reader;
        public BinaryWriter writer;
        public String Name;
        public int Punktzahl;

        public Spieler(TcpClient Verb, String name)
        {
            Cl = Verb;
            ClSt = Cl.GetStream();
            reader = new BinaryReader(ClSt);
            writer = new BinaryWriter(ClSt);
            Name = name;
        }

        public void SendText(String nachricht)
        {
            writer.Write(nachricht);
            reader.ReadBoolean();
        }

        public void SendNumber(Int64 ID)
        {
            writer.Write(ID);
            reader.ReadBoolean();
        }

        public void SendBool(bool Wert)
        {
            writer.Write(Wert);
            reader.ReadBoolean();
        }

        public bool ReadBoolean()
        {
            bool Data = reader.ReadBoolean();
            writer.Write(true);
            return Data;
        }

        public string ReadStringNachricht()
        {
            string Data = reader.ReadString();
            writer.Write(true);
            return Data;
        }

        public int ReadInt64()
        {
            int Data = (int)reader.ReadInt64();
            writer.Write(true);
            return Data;
        }

        public void AddPunkteVonStich(int Punkte)
        {
            Punktzahl += Punkte;
        }
    }
}
