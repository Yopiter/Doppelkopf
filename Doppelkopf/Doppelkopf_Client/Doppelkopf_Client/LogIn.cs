using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Doppelkopf_Client
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
            if (name == "Martin") name = "The Creator";
            int Port = (int)NB_Port.Value;

            if (!IPAddress.TryParse(IP, out IPAddress Adresse))
            {
                LB_Status.Text = "Ungültige IP";
                return;
            }
            Host = new TcpClient();
            Host.Connect(Adresse, Port);
            BinaryReader r = new BinaryReader(Host.GetStream());
            BinaryWriter w = new BinaryWriter(Host.GetStream());
            w.Write(name);
            if (r.ReadString() != "Roger Roger")
            {
                LB_Status.Text = "Kommunikation fehlgeschlagen. Name möglicherweise bereits vergeben";
                Host.Close();
                return;
            }
            CName = name;
            Close();
        }
    }
}
