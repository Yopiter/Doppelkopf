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

        const string ANSWER_SUCCESS = "Roger Roger :D"; //DLL

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
            try
            {
                Host.Connect(Adresse, Port);
            }
            catch(Exception ex)
            {
                LB_Status.Text = "Keine Verbindung möglich: " + ex.Message;
                return;
            }
            BinaryReader r = new BinaryReader(Host.GetStream());
            BinaryWriter w = new BinaryWriter(Host.GetStream());
            w.Write(name);
            if (r.ReadString() != ANSWER_SUCCESS)
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
