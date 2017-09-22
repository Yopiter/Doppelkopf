using System;
using System.Collections.Generic;

namespace Doppelkopf_Server
{
    public partial class Program
    {
        private const string NACHRICHT_AMZUG = "Du Du Du Du bist dran"; //DLL
        static List<Stich> StichListe;

        #region StringListen //DLL
        static string[] NachrichtenListeHochzeit = { "Läutet die Glocken! {0} und {1} sind von nun an glücklich verheiratet!", "Die dunkle Allianz aus {0} und {1} hat sich erhoben!", "Bauer sucht Frau kommt zu Happy End: {0} und {1} sind zusammen, bis dass das Spielende sie scheide!", "Batman Forever: Batman ({0}) und Robin({1}) versuchen zusammen, das Spiel zu retten!", "HP und der Feuerkelch: Der dunkle Lord ({0}) hat sich aus seinem Grab erhoben und plant mit seinem treuesten Diener ({1}), diese Runde für sich zu erobern!", "Iiih, schau mal, {0} und {1} machen Hochzeitsnacht!" };
        static string[] ServerSendeSprueche = { "Server sagt: ", "Serverus Snape sagte:", "Und Gott sprach: ", "Martins Minion meinte: ", "Hört, Hört:", "Höret her und lasst euch sagen: ", "Also: ", "Spacko sagt: ", "Michovski Meerschwein meckerte: " };
        static string[] NachrichtenListeKeineHochzeit = { "Irgendwer, Eventuell {0}, hat es gerade richtig verkackt :D", "Achievement unlocked - 'Forever Alone'! -1 auf Teamstärke für alle Spieler mit em Namen {0}!", "Forever Single 2 - In der Hauptrolle: {0}", "Nein {0}, du bist nicht dick, aber du kommst viel besser rüber, wenn du dich in der Öffentlichkeit von mir fernhältst...", "Die Antwort ist NEIN! Für den Versuch gibts aber trotztdem 3,14159265 Punkte an Griffindor und eine Einstweilige Verfügung für {0}." };
        static string[] NachrichtenListeEmo = { "Wow, {0} hast du die Runde überhaupt schon was gerissen?", "Schäm dich {0}", "Alle mit Namen {0} und {1} sollten den nächsten Stich blind spielen. Dann hat das Glück bessere Chancen", "Spieler {0} und {1} wurden durch Bots ersetzt, um ihren Teammitgliedern wenigstens eine Chance auf den Sieg zu bieten." };
        static string[] NachrichtenListeStartspieler = { "Der/Die/Das größte Opfer beginnt: {0]", "Der Spieler mit den meisten 3. Zähnen beginnt: {0}", "Gottimperator {0} beginnt", "Spackenstart: {0}", "{0} der Fabulöse spielt den Auftakt", "Anstoß für {0}", "{0} kommt wie immer zuerst (That's what she said...)", "{0} beginnt, doch Schwarz gewinnt!" };
        #endregion

        /// <summary>
        /// Allgemeine Funktion zu Beginn einer Runde
        /// </summary>
        /// <returns>Zufälliger Index des Startspielers</returns>
        static int AblaufStarten()
        {
            StichListe = new List<Stich>();
            Random RanInt = new Random((int)DateTime.Now.Ticks);
            int SpPosition = RanInt.Next(4); //Zufällige Bestimmung des Startspielers!

            BroadcastStartspieler(SpPosition);

            return SpPosition;
        }

        #region NormalerAblauf

        static void AblaufNormal()
        {
            int SpPosition = AblaufStarten();

            List<Spieler> Reihenfolge = new List<Spieler>();
            for (int Stichzahl = 0; Stichzahl < 12; Stichzahl++) //12 Stiche pro Spiel
            {
                //Reihenfolge festlegen
                Reihenfolge.Clear();
                for (int x = 0; x < 4; x++)
                {
                    int z = x + SpPosition;
                    if (z > 3)
                        z -= 4;
                    Reihenfolge.Add(SpielerListe[z]);
                }

                Stich CurStich = new Stich(Reihenfolge);
                for (int i = 0; i < 4; i++)         //Innerhalb eines Stiches
                {
                    Reihenfolge[i].SendText(NACHRICHT_AMZUG);  //Startspieler Erlaubnis erteilen
                    int ID = Reihenfolge[i].ReadInt64();
                    CurStich.KarteGespielt(Deck[ID]);   //Karte eintragen
                    int SpielerID = SpPosition + i > 3 ? SpPosition + i - 4 : SpPosition + i;
                    BroadcastCard(ID, SpielerID);  //Karte broadcasten
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
            int SpPosition = AblaufStarten();
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
                    Reihenfolge[i].SendText(NACHRICHT_AMZUG);  //Startspieler Erlaubnis erteilen
                    int ID = Reihenfolge[i].ReadInt64();
                    CurStich.KarteGespielt(Deck[ID]);   //Karte eintragen
                    int SpielerID = SpPosition + i > 3 ? SpPosition + i - 4 : SpPosition + i;
                    BroadcastCard(ID, SpielerID);  //Karte broadcasten
                }
                StichListe.Add(CurStich);
                SpPosition = SpielerListe.IndexOf(CurStich.SpielerGingAn);
                Console.WriteLine("Stich mit den folgenden Punkten geht an: " + CurStich.SpielerGingAn.Name + " mit " + CurStich.StichPunktwert + " Punkten");
                BroadcastStich(CurStich);
                if (Stichzahl < 3 && CurrentGame.TeamRe.Count == 1)
                {
                    if (IsHochzeitsStich(CurStich))
                    {
                        Spieler spielerGingAn = CurStich.SpielerGingAn;
                        CurrentGame.TeamRe.Add(spielerGingAn);
                        Console.WriteLine(string.Format("Hochzeit!!! Neuzugang: {0}; Brautpartner: {1}", spielerGingAn.Name, CurrentGame.TeamRe[0].Name));
                        BroadcastHochzeit(spielerGingAn);
                    }
                }
                if (Stichzahl == 2 && CurrentGame.TeamRe.Count == 1)
                {
                    string Nachricht = string.Format(GetRandomStringFromArrayOrList(NachrichtenListeKeineHochzeit), CurrentGame.TeamRe[0].Name);
                    BroadcastMessage(GetRandomStringFromArrayOrList(ServerSendeSprueche) + Nachricht);
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
            AblaufNormal();
        }
    }
}
