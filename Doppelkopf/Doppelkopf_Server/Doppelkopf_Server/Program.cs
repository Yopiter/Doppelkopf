using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Doppelkopf_Server
{
    public partial class Program
    {
        static List<Karte> Deck;
        static List<Spieler> SpielerListe;
        static IPAddress ServerIP;
        static int PortNummer;
        static List<List<Karte>> Spielerkarten; //TODO in Spieler verschieben
        static Spiel CurrentGame;
        const string ANSWER_SUCCESS = "Roger Roger :D"; //DLL
        const string ANSWER_FAILIURE = "Move bitch get out the way.";//DLL

        static void Main(string[] args)
        {
            //Initialisiere Server
            Console.WriteLine("Geben sie eine IP-Adresse an oder lassen sie die Eingabe frei, um auf beliebigen IPs zu hören.");
            string ipString = Console.ReadLine();
            ServerIP = ipString == "" ? IPAddress.Any : IPAddress.Parse(ipString);
            Console.WriteLine("Geben sie den zu verwendenden Port an oder geben sie nichts ein, um den Standardport 666 zu verwenden.");
            string portString = Console.ReadLine();
            PortNummer = portString == "" ? 666 : int.Parse(portString);

            //Verbindungsaufbau
            DeckGenerieren();
            GetSpielerAnmeldungen();

            Console.WriteLine("Alle Spieler sind dem Spiel beigetreten.");
            NamensListenSenden(); //Startsignal für Clients
            KartenAusgeben();

            SpielmodusBestimmen(); //Bisher nur normaler Ablauf integriert
            SpielmodusStarten();
        }

        /// <summary>
        /// Erstellt einen unsortierten Doppelkopfkartensatz.
        /// </summary>
        static void DeckGenerieren()
        {
            Deck = new List<Karte>();
            
            for (int Farbe = 0; Farbe < 4; Farbe++)
            {
                for (int Wert = 0; Wert < 6; Wert++)
                {
                    Karte newCard = new Karte(Farbe, Wert, 0);
                    Deck.Add(newCard);
                    newCard = new Karte(Farbe, Wert, 1);
                    Deck.Add(newCard);
                }
            }
        }

        /// <summary>
        /// Registriert 4 Spieler.
        /// </summary>
        private static void GetSpielerAnmeldungen()
        {
            TcpListener TcpLis = new TcpListener(ServerIP, PortNummer);
            TcpLis.Start();
            Console.WriteLine("TcpListener gestartet. Warte auf Clients");
            SpielerListe = new List<Spieler>();
            for (int i = 0; i < 4; i++)
            {
                Spieler newPlayer = SucheSpieler(TcpLis);
                if (newPlayer == null)
                {
                    i--;
                    continue;
                }
                SpielerListe.Add(newPlayer);
            }
        }

        /// <summary>
        /// Wartet auf eine neue Verbindungsanfrage zum Mitspielen.
        /// </summary>
        /// <param name="TL">Ein Listener auf den Port, über den die Verbindung läuft.</param>
        /// <returns></returns>
        static Spieler SucheSpieler(TcpListener TL)
        {
            TcpClient Ver = TL.AcceptTcpClient();
            BinaryReader r = new BinaryReader(Ver.GetStream());
            String SpielerName = r.ReadString();
            bool akzeptiert = true;

            foreach (Spieler sp in SpielerListe)
            {
                if (sp.Name == SpielerName) akzeptiert = false;
            }

            BinaryWriter w = new BinaryWriter(Ver.GetStream());
            if (akzeptiert)
                w.Write(ANSWER_SUCCESS);
            else
            {
                w.Write(ANSWER_FAILIURE);
                return null;
            }
            Spieler Playa = new Spieler(Ver, SpielerName);
            Console.WriteLine("Client ist beigetreten: " + SpielerName);
            return Playa;
        }

        /// <summary>
        /// Sendet die Liste aller Spielernamen an alle Spieler.
        /// </summary>
        private static void NamensListenSenden()
        {
            foreach (Spieler Playa in SpielerListe)
            {
                Playa.SendText("Namensliste"); //TODO gesammte Kommunikation in const variablen umwandeln und in //DLL
                foreach (Spieler s in SpielerListe)
                {
                    Playa.SendText(s.Name);
                }
                Console.WriteLine("Namensliste gesendet an " + Playa.Name);
            }
        }

        /// <summary>
        /// Generiert aus dem Kartensatz für 4 Spieler Karten und teilt sie den Spielern mit.
        /// </summary>
        private static void KartenAusgeben()
        {
            Spielerkarten = new List<List<Karte>>();
            for (int i = 0; i < 3; i++)
            {
                Spielerkarten.Add(GetRandomHandkarten());
            }
            Spielerkarten.Add(Deck);

            DeckGenerieren();//TODO why?
            for (int i = 0; i < 4; i++)
            {
                HandkartenAnSpielerSenden(Spielerkarten[i], SpielerListe[i]);
            }
        }

        /// <summary>
        /// Sendet einem Spieler eine Liste von (Int64)Karten.id .
        /// </summary>
        /// <param name="KList">Liste der Karten.</param>
        /// <param name="Playa">Spieler an den die Karten gesendet werden.</param>
        private static void HandkartenAnSpielerSenden(List<Karte> KList, Spieler Playa)
        {
            int i = 0;
            Playa.SendText("Karten_Start");
            foreach (Karte K in KList)
            {
                i++;
                Playa.SendNumber((Int64)K.id);
                Console.WriteLine(i.ToString() + " " + K.id.ToString());
            }
            Console.WriteLine("Karten gesendet an " + Playa.Name);
        }


        private static void SpielmodusBestimmen()
        {
            CurrentGame = new Spiel();
            List<int> SpielmodusStimmen = new List<int>();
            foreach (Spieler Playa in SpielerListe)
            {
                Playa.SendText("Spielmodus_Start");
            }
            foreach (Spieler Playa in SpielerListe)
            {
                Playa.SendText("Spielmodus_Abfrage");
                Console.WriteLine("Warte auf Spielmodus von Spieler " + Playa.Name);
                SpielmodusStimmen.Add(Playa.ReadInt64());
                bool Re = Playa.ReadBoolean();
                bool Kontra = Playa.ReadBoolean();
                foreach (Spieler sp in SpielerListe) //Wahl des Spielers an andere Spieler weiterleiten
                {
                    sp.SendText("Spielmodus_Ansage");
                    sp.SendNumber(SpielmodusStimmen[SpielmodusStimmen.Count - 1]);
                    sp.SendBool(Re);
                    sp.SendBool(Kontra);
                }
            }
            //Höchsten Spielmodus und den Spielenden bestimmen
            int Modus = 0;
            int Spielender = 0;
            for (int i = 0; i < 4; i++)
            {
                if (SpielmodusStimmen[i] > Modus)
                {
                    Spielender = i;
                    Modus = SpielmodusStimmen[i];
                }
            }
            switch (Modus)
            {
                case (0): //Normales Spiel
                    CurrentGame.gameMode = Spiel.Spielmodus.Normal;
                    SetBeideTeams(GetSpielerMitDerAlten());
                    if (CurrentGame.TeamRe.Count != 2)
                        ProgrammMitFehlermeldungBeenden("Normales Spiel ausgewählt, aber " + CurrentGame.TeamRe.Count + " Spieler in Team Re?!");
                    break;

                case (1): //Hochzeit
                    //Temporär nur den Besitzer beider Alten als team Re deklarieren und später korrigieren
                    SetBeideTeams(GetSpielerMitDerAlten());
                    CurrentGame.gameMode = Spiel.Spielmodus.Hochzeit;
                    if (CurrentGame.TeamRe.Count != 1)
                        ProgrammMitFehlermeldungBeenden("Hochzeit ausgewählt, aber fehlerhafte Anzahl von Spielern in Team Re: " + CurrentGame.TeamRe.Count);
                    break;

                case (2):
                    CurrentGame.gameMode = Spiel.Spielmodus.StillesSolo;
                    SetBeideTeams(GetSpielerMitDerAlten());
                    if (CurrentGame.TeamRe.Count != 1)
                        ProgrammMitFehlermeldungBeenden("Stilles Solo ausgewählt, aber fehlerhafte Anzahl von Spielern in Team Re: " + CurrentGame.TeamRe.Count);
                    break;

                default:
                    ProgrammMitFehlermeldungBeenden("Unbekannter Spielmodus. Bitte prüfen sie, ob alle Teilnehmer identische Versionen des Programms verwenden! Hacker sind nicht erlaubt!");
                    break;
            }

            //Bekanntgabe des Modus und des Spielenden
            foreach (Spieler sp in SpielerListe)
            {
                sp.SendText("Spielmodus_final");
                sp.SendNumber(Modus);
                sp.SendNumber(Spielender);
            }
        }

        /// <summary>
        /// Setzt Spieler ins Team Re und Kontra.
        /// </summary>
        /// <param name="Team1">Liste der Spieler im Team Re</param>
        static void SetBeideTeams(List<Spieler> Team1)
        {
            if (Team1.Count < 1 || Team1.Count > 3)
            {
                ProgrammMitFehlermeldungBeenden("Fehlerhafte Anzahl an Spielern in Team Re: " + Team1.Count);
            }
            CurrentGame.TeamRe = Team1;
            CurrentGame.TeamKontra = SpielerListe.ToArray().Except(CurrentGame.TeamRe.ToArray()).ToList();
        }

        /// <summary>
        /// Sucht die Spieler, die eine Alte besitzen.
        /// </summary>
        /// <returns>Liste von 'Spieler' mit einer Alten.</returns>
        private static List<Spieler> GetSpielerMitDerAlten()
        {
            List<Spieler> SpielerMitAlten = new List<Spieler>();
            for (int i = 0; i < 4; i++)
            {
                foreach (Karte K in Spielerkarten[i])
                {
                    if (K.IstAlte())
                    {
                        Spieler Playa = SpielerListe[i];
                        if (!SpielerMitAlten.Contains(Playa))
                        {
                            SpielerMitAlten.Add(Playa);
                        }
                    }
                }
            }
            return SpielerMitAlten;
        }

        private static void SpielmodusStarten()
        {
            StichListe = new List<Stich>(); //TODO muss das hier initialisiert werden?
            switch (CurrentGame.gameMode)
            {
                case (Spiel.Spielmodus.Normal):
                    AblaufNormal();
                    break;

                case (Spiel.Spielmodus.Hochzeit):
                    AblaufHochzeit();
                    break;

                case (Spiel.Spielmodus.StillesSolo):
                    AblaufStillesSolo();
                    break;

                default:
                    ProgrammMitFehlermeldungBeenden("Unbekannter Spielmodus zu Spielstart: " + CurrentGame.gameMode.ToString());
                    break;
            }
        }

        /// <summary>
        /// Gibt aus dem aktuellen Deck eine Liste von 12 Handkarten aus.
        /// </summary>
        /// <returns>Liste mit den Handkarten.</returns>
        private static List<Karte> GetRandomHandkarten()
        {
            List<Karte> handkartenListe = new List<Karte>();
            int Index;
            Random randomizer = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 12; i++)
            {
                Index = randomizer.Next(Deck.Count);
                handkartenListe.Add(Deck[Index]);
                Deck.RemoveAt(Index);
            }

            return handkartenListe;
        }


        #region Broadcasts
        private static void BroadcastStich(Stich curStich)
        {
            foreach (Spieler sp in SpielerListe)
            {
                sp.SendText("Stich Punkte und Spieler");
                sp.SendNumber(curStich.StichPunktwert);
                sp.SendNumber(SpielerListe.IndexOf(curStich.SpielerGingAn));
            }
        }

        private static void BroadcastErg()
        {
            foreach (Spieler sp in SpielerListe)
            {
                sp.SendText("TeamRe");
                foreach(Spieler Re in CurrentGame.TeamRe)
                {
                    sp.SendNumber(SpielerListe.IndexOf(Re)); //unbekannt viele, deshalb Abbruch mit -1
                }
                sp.SendNumber(-1);
                sp.SendText("Endergebnis");
                for (int i = 0; i < 4; i++)
                {
                    sp.SendNumber(SpielerListe[i].Punktzahl);
                }
                Console.WriteLine("Ergebnis gesendet an Spieler " + sp.Name);
            }
        }

        private static void BroadcastCard(int KartenID, int SpielerID)
        {
            foreach (Spieler sp in SpielerListe)
            {
                sp.SendText("gespielte Karte");
                sp.SendNumber(KartenID);
                sp.SendNumber(SpielerID);
            }
        }

        private static void BroadcastMessage(string Message)
        {
            foreach (Spieler sp in SpielerListe)
            {
                sp.SendText("NachrichtVomServer");
                sp.SendText(Message);
            }
        }

        private static void BroadcastHochzeit()
        {
            string Nachricht = string.Format(GetRandomStringFromArrayOrList(NachrichtenListeHochzeit), CurrentGame.TeamRe[0].Name, CurrentGame.TeamRe[1].Name);
            BroadcastMessage(GetRandomStringFromArrayOrList(ServerSendeSprueche) + Nachricht);
            foreach (Spieler sp in SpielerListe)
            {
                sp.SendText("Hochzeitspaar <3");
                sp.SendNumber(SpielerListe.IndexOf(CurrentGame.TeamRe[0]));
                sp.SendNumber(SpielerListe.IndexOf(CurrentGame.TeamRe[2]));
            }
        }
        #endregion

        private static void ProgrammMitFehlermeldungBeenden(string Fehlermeldung)
        {
            Console.WriteLine(Fehlermeldung);
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static string GetRandomStringFromArrayOrList(IEnumerable<string> Array)
        {
            Random IndexGenerator = new Random();
            return Array.ElementAt(IndexGenerator.Next(Array.Count()));
        } 
    }
}
