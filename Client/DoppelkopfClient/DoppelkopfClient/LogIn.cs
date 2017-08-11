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

namespace DoppelkopfClient
{
    public partial class LogIn : Form
    {
        public TcpClient Host;
        public String CName;

        public LogIn()
        {
            InitializeComponent();
        }

        private void BT_Verb_Click(object sender, EventArgs e)
        {
            String IP = TB_IP.Text;
            String name = TB_Name.Text;
            if (name == "Laura") name = "Sexy Beast :D";
            int Port = (int)NB_Port.Value;

            IPAddress Adresse;
            if (!IPAddress.TryParse(IP, out Adresse))
            {
                LB_Status.Text = "Ungültige IP";
                return;
            }
            Host = new TcpClient();
            Host.Connect(Adresse, Port);
            BinaryReader r = new BinaryReader(Host.GetStream());
            BinaryWriter w = new BinaryWriter(Host.GetStream());
            w.Write(name);
            if (r.ReadString() != "Roger :D")
            {
                LB_Status.Text = "Kommunikation fehlgeschlagen. Name möglicherweise bereits vergeben";
                Host.Close();
                return;
            }
            CName = name;
            this.Close();
        }
    }
}
