using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DoppelkopfServer
{
    class Program
    {
        static IPAddress IP;
        static int Port;
        static Spieler S1;
        static Spieler S2;
        static Spieler S3;
        static Spieler S4;
        static List<Spieler> ClientList;
        static List<Stich> StichList;
        static List<Karte> Deck;
        static List<Karte> Sp1K;
        static List<Karte> Sp2K;
        static List<Karte> Sp3K;
        static List<Karte> Sp4K;

        static void Main(string[] args)
        {
            Console.WriteLine("Geben sie die IP an");
            IP = IPAddress.Parse(Console.ReadLine());
            Console.WriteLine("Geben sie den Port an");
            Port =int.Parse(Console.ReadLine());
            DeckErstellen();
            TcpListener TL = new TcpListener(IP,Port);
            TL.Start();
            Console.WriteLine("Listener gestartet. Warte auf Clients");
            ClientList = new List<Spieler>();
            SucheSpieler(TL, ref S1);
            ClientList.Add(S1);
            SucheSpieler(TL, ref S2);
            ClientList.Add(S2);
            SucheSpieler(TL, ref S3);
            ClientList.Add(S3);
            SucheSpieler(TL, ref S4);
            ClientList.Add(S4);
            StartBestätigen();
            KartenVerteilen();
            KartenGeben(ref S1, Sp1K);
            KartenGeben(ref S2, Sp2K);
            KartenGeben(ref S3, Sp3K);
            KartenGeben(ref S4, Sp4K);
            StichList = new List<Stich>();
            Ablauf_Normal();
            broadcastErg();
            Console.ReadLine();
        }

        static void KartenGeben(ref Spieler Playa, List<Karte> KList)
        {
            Playa.SendText("Karten Start");
            int i = 0;
            foreach (Karte K in KList)
            {
                Playa.r.ReadBoolean();
                i++;
                Playa.SendNumber((Int64)K.ID);
                Console.WriteLine(i.ToString()+ " " + K.ID.ToString());
            }
            Console.WriteLine("Karten gesendet an "+Playa.Name);
            while (Playa.r.ReadString() != "Roger :D") { }
            foreach (Spieler s in ClientList)
            {
                Playa.r.ReadBoolean();
                Playa.SendText(s.Name);
            }
            Console.WriteLine("Namensliste gesendet an " + Playa.Name);
            Playa.r.ReadBoolean();
        }

        static List<Karte> RandomKarten()
        {
            List<Karte> CurList = new List<Karte>();
            int Index;
            Random randomizer = new Random();
            for (int i = 0; i < 12; i++)
            {
                Index = randomizer.Next(Deck.Count);
                CurList.Add(Deck[Index]);
                Deck.RemoveAt(Index);
            }
            return CurList;
        }

        static void DeckErstellen()
        {
            Deck = new List<Karte>();
            for (int Farbe = 0; Farbe < 4; Farbe++)
            {
                for (int Wert = 0; Wert < 6; Wert++)
                {
                    Karte newCard = new Karte(Farbe, Wert, "",0);
                    Deck.Add(newCard);
                    newCard = new Karte(Farbe, Wert, "", 1);
                    Deck.Add(newCard);
                }
            }
        }

        static void KartenVerteilen()
        {
            DeckErstellen();
            Sp1K = RandomKarten();
            Sp2K = RandomKarten();
            Sp3K = RandomKarten();
            Sp4K = Deck;
            DeckErstellen();
        }

        static void StartBestätigen()
        {
            foreach (Spieler sp in ClientList)
            {
                sp.SendText("Start");
            }
        }

        static void SucheSpieler(TcpListener TL, ref Spieler Playa)
        {
            TcpClient Ver = TL.AcceptTcpClient();
            BinaryReader r = new BinaryReader(Ver.GetStream());
            String Name = r.ReadString();
            bool akzeptiert = true;
            foreach (Spieler sp in ClientList)
            {
                if (sp.Name == Name) akzeptiert = false;
            }
            BinaryWriter w = new BinaryWriter(Ver.GetStream());
            if (akzeptiert) w.Write("Roger :D");
            else
            {
                w.Write("Nope");
                SucheSpieler(TL, ref Playa);
                return;
            }
            Playa = new Spieler(Ver, Name);
            Console.WriteLine("Client ist beigetreten: " + Name);
        }

        static void Ablauf_Normal()
        {
            int SpPosition = 1;
            for (int n = 0; n < 12; n++) //12 Stiche pro Spiel
            {
                List<Spieler> Reihenfolge = new List<Spieler>();
                for (int x = 0; x < 4; x++)
                {
                    int z = x+SpPosition;
                    if (z > 3) z -= 4;
                    Reihenfolge.Add(ClientList[z]);
                }
                Stich CurStich = new Stich(Reihenfolge);
                for (int i = 0; i < 4; i++)         //Innerhalb eines Stiches
                {
                    Reihenfolge[i].SendText("Du Du Du Du bist dran!");  //Startspieler Erlaubnis erteilen
                    int ID = Reihenfolge[i].WaitForCard();
                    CurStich.KarteGespielt(Deck[ID]);   //Karte eintragen
                    broadcastCard(ID,SpPosition+i);  //Stich broadcasten
                }
                StichList.Add(CurStich);
                SpPosition = ClientList.IndexOf(CurStich.GingAn);
            }
            //ende der Stiche - Auswertung/Ergebnisse

        }

        static void broadcastCard(int ID, int SpielerNr)
        {
            foreach (Spieler Sp in ClientList)
            {
                Sp.SendText("gespielte Karte");
                Sp.r.ReadBoolean();
                Sp.SendNumber((Int64) ID);
                Sp.r.ReadBoolean();
                Sp.w.Write((Int16)SpielerNr);
                Sp.r.ReadBoolean();
            }
        }

        static void broadcastMessage(String Message)
        {
            foreach (Spieler Sp in ClientList)
            {
                Sp.SendText("Message");
                Sp.r.ReadBoolean();
                Sp.SendText(Message);
                Sp.r.ReadBoolean();
            }
        }

        static void broadcastErg()
        {
            foreach (Spieler Sp in ClientList)
            {
                Sp.w.Write("Endergebnis");
                Sp.r.ReadBoolean();
                for (int i = 0; i < 4; i++)
                {
                    Sp.SendNumber(ClientList[i].Punktzahl);
                    Sp.r.ReadBoolean();
                }
                Console.WriteLine("Ergebnis an Spieler "+Sp.Name+" gesendet");
            }
        }
    }
}
