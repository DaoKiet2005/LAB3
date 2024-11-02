using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LAB3_BAI3
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private Thread listenThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Listen(object sender, EventArgs e)
        {
            IPAddress ipAddress = IPAddress.Parse(txtIP.Text);
            int port = int.Parse(txtPort.Text);
            tcpListener = new TcpListener(ipAddress, port);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.IsBackground = true;
            listenThread.Start();

            LogMessage("Waiting for a connection...");
        }
        private void ListenForClients()
        {
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                LogMessage("Connected");

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }
        private void HandleClientComm(object client_obj)
        {
            TcpClient tcpClient = (TcpClient)client_obj;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }

                string clientMessage = Encoding.UTF8.GetString(message, 0, bytesRead);
                LogMessage("Client: " + clientMessage);
            }

            tcpClient.Close();
            LogMessage("Client disconnected.");
        }

        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogMessage), message);
                return;
            }

            richTextBoxLog.AppendText(message + Environment.NewLine);
        }
    }
}
