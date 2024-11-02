using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private TcpListener tcpListener;
        private Thread listenThread;
        StringBuilder receivedData = new StringBuilder();

        private void btnListen(object sender, EventArgs e)
        {
            int port = 8080;
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.IsBackground = true;
            listenThread.Start();

            txtStatus.AppendText($"Telnet running on 127.0.0.1:{port}\n");
        }

        private void ListenForClients()
        {
            tcpListener.Start();

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
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
                string receivedMessage = Encoding.UTF8.GetString(message, 0, bytesRead);
                receivedData.Append(receivedMessage);
                int newLineIndex;
                while ((newLineIndex = receivedData.ToString().IndexOf('\n')) != -1)
                {
                    string line = receivedData.ToString().Substring(0, newLineIndex).Trim();
                    receivedData.Remove(0, newLineIndex + 1);

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            txtStatus.AppendText("Received: " + line + "\n");
                        });
                    }

                }
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
    }
}
