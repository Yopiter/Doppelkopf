using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Doppelkopf_Client
{
    public partial class Main : Form
    {
        TcpClient Server;
        string PlName;
        bool verbunden;
        List<Button> KartenButtons;
        List<Button> StichButtons;
        List<Button> LetzterStichButtons;
        List<Karte> VerbleibendeKarten;
        List<Karte> Stiche;
        List<Spieler> SpielerListe;
        List<TextBox> LabelList;

        public Main()
        {
            InitializeComponent();
            SetupTableVariables();
            LogIn AuthFenster = new LogIn();
            AuthFenster.ShowDialog();
            if (AuthFenster.Host == null) Environment.Exit(0);
            Server = AuthFenster.Host;
            PlName = AuthFenster.CName;
            verbunden = true;
            ThreadPool.QueueUserWorkItem(WaitForServerCommands);
        }

        private void SetupTableVariables()
        {
            BackgroundImage = new Bitmap("Ress\\Background.jpg");
            KartenButtons = new List<Button> { K1, K2, K3, K4, K5, K6, K7, K8, K9, K10, K11, K12 };
            StichButtons = new List<Button> { Stich_1, Stich_2, Stich_3, Stich_4 };
            LetzterStichButtons = new List<Button> { L_1, L_2, L_3, L_4 };
            LabelList = new List<TextBox> { NameN1, NameN2, NameN3, NameN4 };
            SpielerListe = new List<Spieler>() { null, null, null, null };
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
                    default:
                        MessageBox.Show("Unbekannter Befehl: " + Nachricht);
                        break;
                }
            }
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
            for (int i = 0; i < 4; i++)
            {
                int pos = (spPos + i) > 3 ? spPos + i - 4 : spPos + i;
                OrderedList.Add(new Spieler(Namensliste[pos], StichButtons[i], LabelList[i], i));
            }
            foreach (Spieler sp in OrderedList)
            {
                SpielerListe[sp.AbsolutePositionAufServer] = sp;
            }
            this.BeginInvoke((Func<bool>)ShowSpieler);
        }

        private bool ShowSpieler()
        {
            foreach (Spieler sp in SpielerListe)
            {
                sp.tbLabel.Text = sp.Name;
            }
            return true; //useless, aber BeginInvoke will es so :/
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
    }
}
