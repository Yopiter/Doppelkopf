//void GetInput(Object StateInfo)
//{
//    TcpClient Client = (TcpClient)StateInfo;
//    BinaryReader Reader = new BinaryReader(Client.GetStream());
//    while (true)
//    {
//        string textnachricht = Reader.ReadString();
//        textBox1.Invoke(new Action(() =>
//        {
//            textBox1.Text = textnachricht;
//        }));
//    }
//}

//private void Form1_Shown(object sender, EventArgs e)
//{
//    TcpListener TL = new TcpListener(IPAddress.Loopback, 666);
//    TL.Start();
//    TcpClient Client = TL.AcceptTcpClient();
//    ThreadPool.QueueUserWorkItem(GetInput, Client);
//}