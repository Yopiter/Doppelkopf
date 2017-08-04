using System;
using System.Collections.Generic;

namespace Doppelkopf_Server
{
    public partial class Program
    {
        static List<Stich> StichListe;

        #region StringListen
        static string[] NachrichtenListeHochzeit = { "Läutet die Glocken! {0} und {1} sind von nun an glücklich verheiratet!", "Die dunkle Allianz aus {0} und {1} hat sich erhoben!", "Bauer sucht Frau kommt zu Happy End: {0} und {1} sind zusammen, bis dass das Spielende sie scheide!", "Batman Forever: Batman ({0}) und Robin({1}) versuchen zusammen, das Spiel zu retten!", "HP und der Feuerkelch: Der dunkle Lord ({0}) hat sich aus dem grab erhoben und plant mit seinem treuesten Diener ({1}), diese Runde für sich zu erobern!" };
        static string[] ServerSendeSprueche = { "Server sagt: ", "Serverus Snape sagte:", "Und Gott sprach: ", "Martins Minnion meinte: ", "Hört, Hört:", "Höret her und lasst euch sagen: ", "Spacko sagt: " };
        static string[] NachrichtenListeKeineHochzeit = { "Irgendwer, Eventuell {0}, hat es gerade richtig verkackt :D", "Achievement unlocked - 'Forever Alone'! -1 auf Teamstärke für alle Spieler mit em Namen {0}!", "Forever Single 2 - In der Hauptrolle: {0}", "Nein schatz, du bist nicht dick, aber kommst viel besser rüber, wenn du dich in der Öffentlichkeit von mir fernhältst..." };
        #endregion

        #region NormalerAblauf

        static void AblaufNormal()
        {
            StichListe = new List<Stich>();
            Random RanInt = new Random();
            int SpPosition = RanInt.Next(4); //Zufällige Bestimmung des Startspielers!
            List<Spieler> Reihenfolge = new List<Spieler>();
            for (int Stichzahl = 0; Stichzahl < 12; Stichzahl++) //12 Stiche pro Spiel
            {
                Reihenfolge.Clear();
                for (int x = 0; x < 4; x++)
                {
                    int z = x + SpPosition;
                    if (z > 3) z -= 4;
                    Reihenfolge.Add(SpielerListe[z]);
                }
                Stich CurStich = new Stich(Reihenfolge);
                for (int i = 0; i < 4; i++)         //Innerhalb eines Stiches
                {
                    Reihenfolge[i].SendText("Du Du Du Du bist dran!");  //Startspieler Erlaubnis erteilen
                    int ID = Reihenfolge[i].ReadInt64();
                    CurStich.KarteGespielt(Deck[ID]);   //Karte eintragen
                    BroadcastCard(ID, SpPosition + i);  //Karte broadcasten
                }
                StichListe.Add(CurStich);
                SpPosition = SpielerListe.IndexOf(CurStich.SpielerGingAn);
                Console.WriteLine("Stich mit den folgenden Punkten geht an: " + CurStich.SpielerGingAn.Name + " mit " + CurStich.StichPunktwert + " Punkten");
                BroadcastStich(CurStich);
            }
            //Ende der Stiche - Auswertung/Ergebnisse
            BroadcastErg();
        }

        #endregion

        #region Hochzeit

        static void AblaufHochzeit()
        {
            StichListe = new List<Stich>();
            Random RanInt = new Random();
            int SpPosition = RanInt.Next(4); //Zufällige Bestimmung des Startspielers!
            List<Spieler> Reihenfolge = new List<Spieler>();
            for (int Stichzahl = 0; Stichzahl < 12; Stichzahl++) //12 Stiche pro Spiel
            {
                Reihenfolge.Clear();
                for (int x = 0; x < 4; x++)
                {
                    int z = x + SpPosition;
                    if (z > 3) z -= 4;
                    Reihenfolge.Add(SpielerListe[z]);
                }
                Stich CurStich = new Stich(Reihenfolge);
                for (int i = 0; i < 4; i++)         //Innerhalb eines Stiches
                {
                    Reihenfolge[i].SendText("Du Du Du Du bist dran!");  //Startspieler Erlaubnis erteilen
                    int ID = Reihenfolge[i].ReadInt64();
                    CurStich.KarteGespielt(Deck[ID]);   //Karte eintragen
                    BroadcastCard(ID, SpPosition + i);  //Karte broadcasten
                }
                StichListe.Add(CurStich);
                SpPosition = SpielerListe.IndexOf(CurStich.SpielerGingAn);
                Console.WriteLine("Stich mit den folgenden Punkten geht an: " + CurStich.SpielerGingAn.Name + " mit " + CurStich.StichPunktwert + " Punkten");
                BroadcastStich(CurStich);
                if (Stichzahl < 3 && CurrentGame.TeamRe.Count == 1)
                {
                    if (IsHochzeitsStich(CurStich))
                    {
                        CurrentGame.TeamRe.Add(CurStich.SpielerGingAn);

                        BroadcastHochzeit();
                    }
                }
                if (Stichzahl == 2 && CurrentGame.TeamRe.Count == 1)
                {
                    string Nachricht = string.Format(GetRandomStringFromArrayOrList(NachrichtenListeKeineHochzeit), CurrentGame.TeamRe[0].Name);
                    BroadcastMessage(GetRandomStringFromArrayOrList(ServerSendeSprueche)+ Nachricht);
                }
            }
            //Ende der Stiche - Auswertung/Ergebnisse
            BroadcastErg();
        }

        private static bool IsHochzeitsStich(Stich aktuellerStich)
        {
            return (aktuellerStich.IsFarbstich() && !CurrentGame.TeamRe.Contains(aktuellerStich.SpielerGingAn));
        }

        #endregion

        static void AblaufStillesSolo()
        {
            throw new NotImplementedException("Stilles Solo noch nicht verfügbar. Bitte besuchen sie unseren Shop und suchen bestellen sie das Solo-DLC 'Forever Alone', um diesen Spielmodus freizuschalten.");
        }
    }
}
