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
        bool AmZug;
        int KartenNr;
        int Karte_Stich;
        Team TeamRe = new Team("Re", 0);
        Team TeamKontra = new Team("Kontra", 1);

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
            KartenNr = 0;
        }

        private void Start()
        {
            //Autostart bei Anzeigen des Hauptfensters
            WaitForStart();
            WaitForCards();
            Thread WarteThread = new Thread(WaitForInfo); //Warten auf Info
            WarteThread.Start();
            Statusleiste.Text = "Warte auf Spielbeginn, Wartethread gestartet";
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

        private void WaitForInfo()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            while (verbunden)
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
                    //Am Zug -> Ablauf unterbrechen bis Eingabe?
                }
                if (Nachricht == "Endergebnis") ErgebnisZeigen(); //Anzeige des Ergebnisses, Ende des Spiels
            }
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

        private void WaitForStart()
        {
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            while (verbunden)
            {
                String Nachricht = r.ReadString();
                if (Nachricht == "Start")
                {
                    return;
                }
            }
        }

        private void WaitForCards()
        {
            //12x Karten
            //4x Spieler
            BinaryReader r = new BinaryReader(HStr);
            BinaryWriter w = new BinaryWriter(HStr);
            KList = new List<Karte>();
            while (r.ReadString() != "Karten Start") { }
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
            int SpielerPos;
            for (int i = 0; i < PList.Count; i++)
            {
                if (PList[i].Name == CName)
                {
                    SpielerPos = i;
                }
            }
            TempSpieler.Add();
            MessageBox.Show("Karten und Spieler erhalten");
            w.Write(true);
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
            Trumpflist.Sort((x, y) => x.Trumpfstärke.CompareTo(y.ID));
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
                PB_List[i].Image = new Bitmap("Ress\\Karten_Template\\"+curCard.Farbwert.ToString()+"\\"+curCard.Wertzahl.ToString()+".png");
                PB_List[i].Visible = true;
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

        }
    }
}
