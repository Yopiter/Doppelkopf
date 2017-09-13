using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Doppelkopf_Client
{
    public partial class Main : Form
    {
        TcpClient Server;
        string PlName;

        List<Button> KartenButtons;
        List<Button> StichButtons;
        List<PictureBox> LetzterStichBoxes;
        List<TextBox> LabelList;

        List<Karte> VerbleibendeKarten;
        List<Karte> GesamtesDeck;

        List<Spieler> SpielerListe;

        List<Spielmodus> ListeGewaehlteSpielmodi;
        List<string> StatusList;

        bool AmZug = false;

        Spielmodus FinalerModus;
        #region KonstantenUndZeugs
        private const int STATUS_NACHRICHTENLÄNGE = 40;
        private const int STATUS_NACHRICHTENANZAHL = 8;
        int picBoxBreite = 74;
        int picBoxHoehe = 56;
        #endregion

        public Main()
        {
            InitializeComponent();
            SetupTableVariables();

            LogIn AuthFenster = new LogIn();
            AuthFenster.ShowDialog();

            if (AuthFenster.Host == null)
                Environment.Exit(0);
            Server = AuthFenster.Host;
            PlName = AuthFenster.CName;
            this.Text = "Doppelkopf - " + PlName;

            ThreadPool.QueueUserWorkItem(WaitForServerCommands);
        }

        private void SetupTableVariables()
        {
            Image background;
            using (var temporaryTmpVariable = new Bitmap("Ress\\Background.jpg"))
            {
                background = new Bitmap(temporaryTmpVariable);
            }
            BackgroundImage = background;

            KartenButtons = new List<Button> { BT_K1, BT_K2, BT_K3, BT_K4, BT_K5, BT_K6, BT_K7, BT_K8, BT_K9, BT_K10, BT_K11, BT_K12 };
            StichButtons = new List<Button> { BT_Stich_1, BT_Stich_2, BT_Stich_3, BT_Stich_4 };
            LetzterStichBoxes = new List<PictureBox> { PB_L_1, PB_L_2, PB_L_3, PB_L_4 };
            LabelList = new List<TextBox> { LBL_Player1, LBL_Player2, LBL_Player3, LBL_Player4 };
            SpielerListe = new List<Spieler>() { null, null, null, null };
            StatusList = new List<string>() { "Spielverlauf" };
        }

        private void WaitForServerCommands(object uselessItem)
        {
            BinaryReader r = new BinaryReader(Server.GetStream());
            BinaryWriter w = new BinaryWriter(Server.GetStream());
            while (true)
            {
                string Nachricht = r.ReadString();
                switch (Nachricht)
                {
                    case ("Namensliste"):
                        GetSpielerVonServer(r, w);
                        break;
                    case ("Karten_Start"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)GetKartenVonServer, r, w);
                        break;
                    case ("Spielmodus_Start"):
                        Invoke((Func<string, bool>)SetStatus, "Ansage von Spielmodi: Warte auf Spieler " + SpielerListe[0].Name);
                        ListeGewaehlteSpielmodi = new List<Spielmodus>();
                        w.Write(true);
                        break;
                    case ("Spielmodus_Abfrage"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)SendPreferredSpielmodus, r, w);
                        break;
                    case ("Spielmodus_Ansage"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)GetSpielmodusAndShowIt, r, w);
                        break;
                    case ("NachrichtVomServer"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)NachrichtHolenUndZeigen, r, w);
                        break;
                    case ("Spielmodus_final"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)FinalenSpielModusLesenUndAnzeigen, r, w);
                        break;
                    case ("Du Du Du Du bist dran"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)OnSelbstAmZug, r, w);
                        break;
                    case ("g"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)GespielteKarteLesen, r, w);
                        break;
                    case ("Stich_Punkte_und_Spieler"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)StichEntfernen, r, w);
                        break;
                    case ("TeamRe"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)GetEndergebnis, r, w);
                        break;
                    case ("Hochzeitspaar <3"):
                        Invoke((Func<BinaryReader, BinaryWriter, bool>)GetHochzeitspartner, r, w);
                        break;
                    default:
                        MessageBox.Show("Unbekannter Befehl: " + Nachricht);
                        w.Write(true);
                        break;
                }
            }
        }

        private bool GetHochzeitspartner(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            Spieler Braut = SpielerListe[ReadInt64(r, w)];
            Controls.Add(Braut.AddZustand(Zustand.HochzeitOhneAlte));
            return true;
        }

        private bool GetEndergebnis(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            List<Spieler> TeamRe = new List<Spieler>();
            while (true)
            {
                int i = ReadInt64(r, w);
                if (i == -1) break;
                TeamRe.Add(SpielerListe[i]);
            }
            int PunkteRe = 0;
            string Team;
            foreach (Spieler sp in SpielerListe)
            {
                sp.Punkte = ReadInt64(r, w);
                if (TeamRe.Contains(sp))
                {
                    Team = "(Re)";
                    PunkteRe += sp.Punkte;
                }
                else
                {
                    Team = "(Kontra)";
                }
                SetStatus(string.Format("{0} {1}: {2}", sp.Name, Team, sp.Punkte));
            }
            
            SetStatus("Punkte Re: " + PunkteRe);
            SetStatus("Punkte Kontra: " + (120 - PunkteRe));
            string SiegerTeam = PunkteRe > 60 ? "Re" : "Kontra";
            SetStatus("Es gewinnt Team " + SiegerTeam);

            switch (FinalerModus) //TODO: In stringlisten umwandeln und randomisiert anzeigen! Außerdem: DLL
            {
                case (Spielmodus.Hochzeit):
                    SetStatus("Die Zeit der Trennung ist gekommen. Spacko 1 und Spacko 2 haben die formale Scheidung eingereicht...");
                    break;
                case (Spielmodus.Normal):
                    SetStatus("Wieder mal ein normales Spiel vorbei. Profitipp: Ihr könntet auch mal einen interessanteren Modus spielen.");
                    break;
                case (Spielmodus.StillesSolo):
                    SetStatus("Vielen Dank für ihren Einkauf bei MüSchn Unlmtd. GbmH und Co. AF. Holen sie sich neben dem 'Forever Alone'-DLC für das Stille Solo doch auch das neue 'I am a proud retard'-Paket für Spielmodi wie 'ohne Neunen' und 'Grand Hand Ouvert'!");
                    break;
            }
            return true;
        }

        private bool StichEntfernen(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            int Stichpunkte = ReadInt64(r, w);
            Spieler GingAn = SpielerListe[ReadInt64(r, w)];
            SetStatus(string.Format("Stich mit {0} Punkten für {1}", Stichpunkte, GingAn.Name));
            int i = 0;
            foreach (PictureBox B in LetzterStichBoxes)
            {
                B.Image = StichButtons[i].Image;
                StichButtons[i].Visible = false;
                B.Visible = true;
                i++;
            }
            return true;
        }

        private bool GespielteKarteLesen(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            int cardID = ReadInt64(r, w);
            int SpielerID = ReadInt64(r, w);
            SpielerListe[SpielerID].KarteLegenLassen(GesamtesDeck[cardID]);
            return true;
        }

        /// <summary>
        /// Spieler muss Karte legen oder ragequiten.
        /// Verschiedene Situationen anhand des Spielmoduses unterscheiden
        /// </summary>
        /// <param name="r"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool OnSelbstAmZug(BinaryReader r, BinaryWriter w)
        {
            //TODO: Buttons mit nicht spielbaren Karten deaktivieren. Vertraue niemals einem Spieler in einem Online-Game
            w.Write(true);
            SetStatus("Du bist dran!");
            AmZug = true;
            return true;
        }

        private bool GetSpielmodusAndShowIt(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            int SpielInt = ReadInt64(r, w);
            ListeGewaehlteSpielmodi.Add((Spielmodus)SpielInt);
            Spieler aktSpieler = SpielerListe[ListeGewaehlteSpielmodi.Count - 1];
            bool Re = ReadBool(r, w);
            bool Kontra = ReadBool(r, w);
            string Nachricht = "Spieler " + aktSpieler.Name + GetSpielmodusString((Spielmodus)SpielInt);
            if (Re)
            {
                Controls.Add(aktSpieler.AddZustand(Zustand.Re));
                Nachricht += " und ein Re";
            }
            if (Kontra)
            {
                Controls.Add(aktSpieler.AddZustand(Zustand.Kontra));
                Nachricht += " und ein Kontra";
            }
            SetStatus(Nachricht + " an.");
            return true;
        }

        private bool FinalenSpielModusLesenUndAnzeigen(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            FinalerModus = (Spielmodus)ReadInt64(r, w);
            Spieler Spielender = SpielerListe[ReadInt64(r, w)];
            switch (FinalerModus)
            {
                case (Spielmodus.Normal): //sneaky sneaky
                case (Spielmodus.StillesSolo):
                    MessageBox.Show("Es wird ein normales Spiel gespielt. Irgendwer fängt an...");
                    break;
                case (Spielmodus.Hochzeit):
                    MessageBox.Show(string.Format("Spieler {0} spielt eine Hochzeit.", Spielender.Name));
                    Controls.Add(Spielender.AddZustand(Zustand.HochzeitAlte));
                    break;
                default:
                    ProgrammMitFehlerBeenden("Es wird ein Spielmodus gespielt, der noch nicht erfunden wurde. Der Verdacht auf Zeitreisende wurde bestätigt.");
                    break;
            }
            return true;
        }

        private void ProgrammMitFehlerBeenden(string nachricht)
        {
            MessageBox.Show(nachricht);
            //TODO: Awesome Animation fürs Beenden zeigen, bsp: Alle Karten rot blinken lassen oder so.
            //Wenn schon Fehler, dann richtig :D
            Environment.Exit(0);
        }

        private bool NachrichtHolenUndZeigen(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            string nachricht = ReadString(r, w);
            SetStatus(nachricht);
            return true;
        }

        private bool SendPreferredSpielmodus(BinaryReader r, BinaryWriter w)
        {
            w.Write(true);
            FensterSpielmodus ModusWahl = new FensterSpielmodus(VerbleibendeKarten);
            ModusWahl.ShowDialog();
            int i = (int)ModusWahl.ChosenMode;
            bool ansageRe = ModusWahl.Re;
            bool ansageKontra = ModusWahl.Kontra;
            ModusWahl.Dispose();
            SendNumber(r, w, i);
            SendBool(r, w, ansageRe);
            SendBool(r, w, ansageKontra);
            return true;
        }

        /// <summary>
        /// Informiert Spieler über aktuelle Spielphase
        /// </summary>
        /// <param name="Status">Nachrichtenstring</param>
        /// <returns></returns>
        private bool SetStatus(string Nachricht)
        {
            if (Nachricht.Length > STATUS_NACHRICHTENLÄNGE)
            {
                SetStatus(Nachricht.Substring(0, STATUS_NACHRICHTENLÄNGE));
                SetStatus(Nachricht.Substring(STATUS_NACHRICHTENLÄNGE));
                return true;
            }
            if (StatusList.Count > STATUS_NACHRICHTENANZAHL)
            {
                StatusList.RemoveAt(1); //Oberste Nachticht nicht löschen, ist Überschrift
            }
            StatusList.Add(Nachricht);
            LbStatus.Text = string.Join(Environment.NewLine, StatusList);
            return true;
        }

        /// <summary>
        /// Über Invoke aufgerufen
        /// </summary>
        /// <param name="r"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        private bool GetKartenVonServer(BinaryReader r, BinaryWriter w)
        {
            List<Karte> Deck = DeckGenerieren();
            GesamtesDeck = Deck;
            VerbleibendeKarten = new List<Karte>();
            w.Write(true); //Beginn der Kartenübertragung
            for (int kartenzahl = 0; kartenzahl < 12; kartenzahl++)
            {
                int ID = ReadInt64(r, w);
                VerbleibendeKarten.Add(Deck[ID]);
            }
            SetStatus("Karten erhalten");
            KartenSortierenUndAnzeigen();
            return true;
        }

        private void KartenSortierenUndAnzeigen()
        {
            KartenSortieren();
            foreach (Karte curCard in VerbleibendeKarten)
            {
                string cardpath = curCard.GetImagePath();
                string basepath = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(basepath, cardpath);
                KartenButtons[VerbleibendeKarten.IndexOf(curCard)].Image = Image.FromFile(path);
                KartenButtons[VerbleibendeKarten.IndexOf(curCard)].Visible = true;
                KartenButtons[VerbleibendeKarten.IndexOf(curCard)].Refresh();
            }
        }

        private void KartenSortieren()
        {
            VerbleibendeKarten.Sort((x, y) => x.id.CompareTo(y.id));
            List<Karte> Trumpflist = new List<Karte>();
            List<Karte> Farblist = new List<Karte>();

            foreach (Karte Kar in VerbleibendeKarten)
            {
                if (Kar.trumpfstärke != -1)
                    Trumpflist.Add(Kar);
                else
                    Farblist.Add(Kar);
            }

            if (Trumpflist.Count > 1) Trumpflist.Sort((x, y) => x.trumpfstärke.CompareTo(y.trumpfstärke));
            Trumpflist.AddRange(Farblist);
            VerbleibendeKarten = Trumpflist;
        }

        private void GetSpielerVonServer(BinaryReader r, BinaryWriter w)
        {
            w.Write(true); //Teil von Sendtext!
            List<string> Namensliste = new List<string>();
            List<Spieler> OrderedList = new List<Spieler>();
            for (int i = 0; i < 4; i++)
            {
                Namensliste.Add(ReadString(r, w));
            }
            int spPos = Namensliste.IndexOf(PlName) + 1;
            Size PBSize = new Size(picBoxBreite, picBoxHoehe);
            for (int i = 0; i < 4; i++)
            {
                int pos = (spPos + i) > 3 ? spPos + i - 4 : spPos + i;
                Spieler newSpieler = new Spieler(Namensliste[pos], StichButtons[i], LabelList[i], pos, (PbAnornung)i);
                newSpieler.SetPbSize(PBSize);
                OrderedList.Add(newSpieler);
            }
            foreach (Spieler sp in OrderedList)
            {
                Invoke((Func<Spieler, bool>)AddSpielerToSpielerList, sp);
            }
            BeginInvoke((Func<bool>)ShowSpieler);
        }

        private bool AddSpielerToSpielerList(Spieler sp)
        {
            SpielerListe[sp.AbsolutePositionAufServer] = sp;
            return true;
        }

        private bool ShowSpieler()
        {
            foreach (Spieler sp in SpielerListe)
            {
                sp.tbLabel.Text = sp.Name;
            }
            return true;
        }

        private string ReadString(BinaryReader r, BinaryWriter w)
        {
            string nachricht = r.ReadString();
            w.Write(true);
            return nachricht;
        }

        private int ReadInt64(BinaryReader r, BinaryWriter w)
        {
            int zahl = (int)r.ReadInt64();
            w.Write(true);
            return zahl;
        }

        private bool ReadBool(BinaryReader r, BinaryWriter w)
        {
            bool wert = r.ReadBoolean();
            w.Write(true);
            return wert;
        }

        private void SendString(BinaryReader r, BinaryWriter w, string nachricht)
        {
            w.Write(nachricht);
            r.ReadBoolean();
        }

        private void SendNumber(BinaryReader r, BinaryWriter w, int Nummer)
        {
            w.Write((Int64)Nummer);
            r.ReadBoolean();
        }

        private void SendBool(BinaryReader r, BinaryWriter w, bool wert)
        {
            w.Write(wert);
            r.ReadBoolean();
        }

        private List<Karte> DeckGenerieren()
        {
            List<Karte> Deck = new List<Karte>();
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
            return Deck;
        }

        private string GetSpielmodusString(Spielmodus Mod)
        {
            switch (Mod)
            {
                case (Spielmodus.Normal):
                case (Spielmodus.StillesSolo): //Stilles Solo heist still, weil es nicht angesagt wird -> undercover-Solo :D
                    return " sagt ein normales Spiel";
                case (Spielmodus.Hochzeit):
                    return " sagt eine Hochzeit";
                default:
                    return " sagt einen unbekannten Spielmodus an, der schon lange zu einer Fehlermeldung hätte führen sollen. Bitte versuchen sie nicht mehr das Spiel zu hacken!";
            }
        }

        private void BT_Karte_Click(object sender, EventArgs e)
        {
            if (!AmZug)
            {
                SetStatus("Du bist nicht dran...");
                return;
            }
            AmZug = false;
            Button Trigger = (Button)sender;
            int Kartenposition = KartenButtons.IndexOf(Trigger);
            Karte gespielteKarte = VerbleibendeKarten[Kartenposition];
            VerbleibendeKarten.Remove(gespielteKarte);
            KartenButtons.Remove(Trigger);
            Trigger.Dispose();
            KarteSpielen(gespielteKarte);
        }

        private void KarteSpielen(Karte curCard)
        {
            BinaryReader r = new BinaryReader(Server.GetStream());
            BinaryWriter w = new BinaryWriter(Server.GetStream());
            SendNumber(r, w, curCard.id);
        }

        public static Image GetImage(string path)
        {
            using (var LastOfTheTempVars = new Bitmap(path))
                return new Bitmap(LastOfTheTempVars);
        }
    }
}
