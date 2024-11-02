using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        UdpClient udpServer;

        private void listen(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(txtPort.Text, out int port) && port > 0 && port <= 65535)
                {
                    udpServer = new UdpClient(port);

                    udpServer.BeginReceive(new AsyncCallback(ReceiveCallback), null);
                    MessageBox.Show("Server đã bắt đầu lắng nghe trên cổng " + port.ToString());
                }
                else
                {
                    MessageBox.Show("Cổng không hợp lệ. Vui lòng nhập lại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khởi động server: " + ex.Message);
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpServer.EndReceive(ar, ref clientEndpoint);

            string receivedMessage = Encoding.UTF8.GetString(receivedData).Replace("\r", "").Replace("\n", "");

            string clientIP = clientEndpoint.Address.ToString();

            this.Invoke((MethodInvoker)delegate
            {
                richTextBox1.AppendText($"{clientIP}:{receivedMessage.Trim()}\n");
            });

            udpServer.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

    }
}
