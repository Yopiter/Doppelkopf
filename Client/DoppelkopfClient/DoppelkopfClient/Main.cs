using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace DoppelkopfClient
{
    public partial class Main : Form
    {
        TcpClient Host;
        Stream HStr;
        bool verbunden;
        List<Karte> KList;
        List<Karte> Deck;
        String CName;
        List<Spieler> PList;
        List<Button> PB_List;
        List<Button> Boxen_letzte;
        bool AmZug = false;
        int Karte_Stich;
        Team TeamRe = new Team("Re", 0);
        Team TeamKontra = new Team("Kontra", 1);
        String Spielmodus;
        Thread WarteThread;
        public delegate void LabelDelegate(string Text);
        public delegate void ButtonDelegate(Button BT, Image img);
        public delegate void SichtbarDelegate(Button BT, bool sichtbar);

        public Main()
        {
            InitializeComponent();
            this.BackgroundImage = new Bitmap("Ress\\Background.jpg");
            //DeckErstellen();             //Für Offline-Tests
            //KartenRaten();
            //AnzeigeInitialisieren();
            LogIn LIFenster = new LogIn();
            LIFenster.ShowDialog();
            Host = LIFenster.Host;
            CName = LIFenster.CName.ToString();
            HStr = Host.GetStream();
            LIFenster.Dispose();
            verbunden = true;
            Karte_Stich = 0;
            DeckErstellen();
            Boxen_letzte = new List<Button>() { L_1, L_2, L_3, L_4 };
            AmZug = false;
        }

        private void Start()
        {
            //Autostart bei Anzeigen des Hauptfensters
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork+=(sender, args)=>
            {//Asynchrones Abfragen der Werte
                WaitForStart();
            };
            bw.RunWorkerCompleted += (sender, args) =>
            {//Ausführung im GUI-Thread
                BinaryWriter w = new BinaryWriter(HStr);
                AnzeigeInitialisieren();
                Statusleiste.Text = "Anzeige initialisiert";
                w.Write(true);
                Thread StartThread = new Thread(WaitForBegin);
                StartThread.Start();
            };
            bw.RunWorkerAsync();
            Statusleiste.Text = "Warte auf Spielbeginn, Startthread gestartet";
        }

        private void DeckErstellen()
        {
            Deck = new List<Karte>();
            for (int Farbe = 0; Farbe < 4; Farbe++)
            {
                for (int Wert = 0; Wert < 6; Wert++)
                {
                    Karte newCard = new Karte(Farbe, Wert,0);
                    Deck.Add(newCard);
                    newCard = new Karte(Farbe, Wert, 1);
                    Deck.Add(newCard);
                }
            }
        }

        private void WaitForBegin()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            String Nachricht = "";
            StatusAkt("Warte auf Spielmodus");
            while (Nachricht != "Spielmodus")
            {
                Nachricht = r.ReadString();
                //evtl. Zwischenmeldungen, sollten nicht kommen bisher
            }
            w.Write(true);
            String Spiel_bisher = r.ReadString();
            StatusAkt("SpielmodusAlt erhalten");
            //Zeigen eines Auswahlfeldes für die Spielmodi, beeinflusst vom bisherigen Spielmodus
            Modus_Auswahl Auswahl = new Modus_Auswahl(Spiel_bisher);
            Auswahl.Show();
            w.Write(Auswahl.NeuerModus);
            StatusAkt("Spielmodus vorgeschlagen");
            Spielmodus = r.ReadString();
            w.Write(true);
            StatusAkt("finalen Spielmodus erhalten");
            WarteThread = new Thread(WaitForInfo); //Warten auf Info nach Festlegung des Spielmodus
            WarteThread.Start();
            w.Write(true);
        }

        private void WaitForInfo()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            while (!AmZug)
            {
                String Nachricht = r.ReadString();
                if (Nachricht == "gespielte Karte")
                {
                    KarteGespielt();
                }
                if (Nachricht == "Message")
                {
                    ReadMessage();
                }
                if (Nachricht == "Du Du Du Du bist dran!")
                {
                    //Am Zug -> mach was
                    AmZug = true;
                }
                if (Nachricht == "Endergebnis") ErgebnisZeigen(); //Anzeige des Ergebnisses, Ende des Spiels
            }
            WarteThread.Join();
        }

        private void KarteSpielen(int Position)
        {
            Int64 ID = KList[Position].ID;
            //Karte prüfen (optional)

            //Karte löschen
            PB_List[Position].Visible = false;

            //Karte schicken
            BinaryWriter w = new BinaryWriter(HStr);
            w.Write(ID);
            //Wartethread wieder starten
            WarteThread.Start();
        }

        private void StichWegräumen()
        {
            for (int i = 0; i < 4; i++)
            {
                Boxen_letzte[i].Image = PB_List[i].Image;
                PB_List[i].Visible = false;
            }
        }

        private void KarteGespielt()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);

            w.Write(true);
            int Card_ID = (int) r.ReadInt64();
            w.Write(true);
            int SpielerNr = (int) r.ReadUInt16();
            if (SpielerNr > 3) SpielerNr = SpielerNr - 4;
            PList[SpielerNr].KarteGelegt(Deck[Card_ID]);
            if (Deck[Card_ID].Name == "Alte")
            {
                TeamRe.AddSpieler(PList[SpielerNr]);
                PList[SpielerNr].Alte = true;
                //Anzeige Re?
            }
            Statusleiste.Text = PList[SpielerNr].Name + " legt eine/n " + Deck[Card_ID].Name;
            Karte_Stich++;
            w.Write(true);
            if (Karte_Stich == 4)
            {
                StichWegräumen();
            }
        }

        private void ReadMessage()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);

            w.Write(true);
            String Nachricht = r.ReadString();
            //Anzeige realisieren
            w.Write(true);
        }

        #region Threadsicher
        private void StatusAkt(String Nachricht)
        {
            try
            {
                Statusleiste.BeginInvoke(new LabelDelegate(Label_change), Nachricht);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + " " + ex.TargetSite.ToString()); }
        }

        private void Label_change(String Text)
        {
            Statusleiste.Text = Text;
        }

        private void BtSicht(Button BT, bool sichtbar)
        {
            BT.Visible = sichtbar;
        }

        private void BildÄndern(Button BT, Image img)
        {
            BT.Image = img;
        }
        #endregion

        private void WaitForStart()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            StatusAkt("Warte auf Start");
            while (verbunden)
            {
                String Nachricht = r.ReadString();
                if (Nachricht == "Start")
                {
                    break;
                }
            }
            StatusAkt("Start bestätigt, Warte auf Karten");
            WaitForCards();
        }

        private void WaitForCards()
        {
            //12x Karten
            //4x Spieler
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            KList = new List<Karte>();
            while (r.ReadString() != "Karten Start") { }
            StatusAkt("Kartenübertragung beginnt");
            for (int i = 0; i < 12; i++)
            {
                w.Write(true);
                int Nr = (int)r.ReadInt64();
                KList.Add(Deck[Nr]);
            }
            PList = new List<Spieler>();
            w.Write("Roger :D");
            for (int i = 0; i < 4; i++)
            {
                w.Write(true);
                String PName = r.ReadString();
                Spieler nSp = new Spieler(PName, i);
                PList.Add(nSp);
            }
            //relative Positionen zum Anwender zuweisen
            List<Spieler> TempSpieler = new List<Spieler>();
            int SpielerPos = 0;
            for (int i = 0; i < PList.Count; i++)
            {
                if (PList[i].Name == CName)
                {
                    SpielerPos = i;
                }
            }
            int Pos;
            for (int i = SpielerPos + 1; i < SpielerPos + 4; i++)
            {
                if (i >= PList.Count) Pos = i - 4;
                else Pos = i;
                TempSpieler.Add(PList[Pos]);
            }
            TempSpieler[0].BoxZuweisen(Stich_2, NameN1);
            TempSpieler[1].BoxZuweisen(Stich_3, NameN2);
            TempSpieler[2].BoxZuweisen(Stich_4, NameN3);
            StatusAkt("Kartenübertragung abgeschlossen");
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            Start();
        }

        private void KartenRaten()
        {
            KList = new List<Karte>();
            Random randomizer = new Random();
            for (int i = 0; i < 12; i++)
            {
                int ID=randomizer.Next(Deck.Count);
                KList.Add(Deck[ID]);
            }
        }

        private void KartenSortieren()
        {
            KList.Sort((x, y) => x.ID.CompareTo(y.ID));
            List<Karte> Trumpflist = new List<Karte>();
            List<Karte> Farblist = new List<Karte>();
            foreach (Karte Kar in KList)
            {
                if (Kar.Trumpfstärke!=-1) { Trumpflist.Add(Kar); }
                else {Farblist.Add(Kar);}
            }
            if (Trumpflist.Count>1) Trumpflist.Sort((x, y) => x.Trumpfstärke.CompareTo(y.ID));
            Trumpflist.AddRange(Farblist);
            KList = Trumpflist;
        }

        private void AnzeigeInitialisieren()
        {
            PB_List = new List<Button>() { K1, K2, K3, K4, K5, K6, K7, K8, K9, K10, K11, K12 };
            KartenSortieren();
            for (int i = 0; i < 12; i++)
            {
                Karte curCard = KList[i];
                //Threadsicher...
                //PB_List[i].BeginInvoke(new ButtonDelegate(BildÄndern), new object[] { PB_List[i], new Bitmap("Ress\\Karten_Template\\" + curCard.Farbwert.ToString() + "\\" + curCard.Wertzahl.ToString() + ".png") });
                //PB_List[i].BeginInvoke(new SichtbarDelegate(BtSicht), true);
                PB_List[i].Image=new Bitmap("Ress\\Karten_Template\\" + curCard.Farbwert.ToString() + "\\" + curCard.Wertzahl.ToString() + ".png");
                PB_List[i].Visible=true;
            }
            foreach (Spieler Sp in PList)
            {
                Sp.NameSchreiben();
            }
        }

        

        private void ErgebnisZeigen()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);

            w.Write(true);
            for (int i = 0; i < 4; i++)
            {
                PList[i].Punkte = (int)r.ReadInt64();
                w.Write(true);
            }
            int PunkteRe = TeamRe.GesamtPunkte();
            int PunkteKontra = 120 - PunkteRe;
            //Verarbeitung der Ergebnisse, Anzeige irgendwie...
            String[] InfoRe = { };
            String[] InfoKontra = { };
            ScoreBoard Erg_Anzeige = new ScoreBoard(InfoRe,InfoKontra,Spielmodus);
            Erg_Anzeige.Show();
        }

        #region Karten_Events
        private void K1_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(0);
            }
        }

        private void K2_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(1);
            }
        }

        private void K3_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(2);
            }
        }

        private void K4_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(3);
            }
        }

        private void K5_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(4);
            }
        }

        private void K6_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(5);
            }
        }

        private void K7_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(6);
            }
        }

        private void K8_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(7);
            }
        }

        private void K9_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(8);
            }
        }

        private void K10_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(9);
            }
        }

        private void K11_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(10);
            }
        }

        private void K12_Click(object sender, EventArgs e)
        {
            if (AmZug)
            {
                KarteSpielen(11);
            }
        }
        #endregion
    }
}
