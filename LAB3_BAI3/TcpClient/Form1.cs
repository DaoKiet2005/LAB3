using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TcpClient
{
    public partial class Form1 : Form
    {
        private System.Net.Sockets.TcpClient client;
        private NetworkStream clientStream;
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_ketnoi(object sender, EventArgs e)
        {
            try
            {
                client = new System.Net.Sockets.TcpClient(txtIP.Text, int.Parse(txtPort.Text));
                clientStream = client.GetStream();

                var readThread = new System.Threading.Thread(ReadData);
                readThread.IsBackground = true;
                readThread.Start();
            }
            catch (Exception ex)
            {
                LogMessage("Error: " + ex.Message);
            }
        }
        
        private void btn_send(object sender, EventArgs e)
        {
            if (client != null && clientStream != null)
            {
                try
                {
                    string message = txtMessage.Text;
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();

                    LogMessage(message);
                    txtMessage.Clear();
                }
                catch (Exception ex)
                {
                    LogMessage("Error: " + ex.Message);
                }
            }
            else
            {
                LogMessage("Not connected to server.");
            }
        }
        private void ReadData()
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        LogMessage("Disconnected from server.");
                        break;
                    }

                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    LogMessage("Server: " + message);
                }
                catch
                {
                    LogMessage("Connection lost.");
                    break;
                }
            }
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
