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

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private UdpClient udpClient;

        private void btn_send(object sender, EventArgs e)
        {
            try
            {
                if (udpClient == null)
                {
                    udpClient = new UdpClient();
                }

                string ipAddress = txtIPAddress.Text;
                int port = int.Parse(txtPort.Text);

                IPAddress ip;
                if (!IPAddress.TryParse(ipAddress, out ip))
                {
                    MessageBox.Show("IP Address chưa đúng. Vui lòng nhập lại.");
                    return;
                }

                string message = txtMessage.Text;
                if (string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Message Trống. Vui lòng nhập nội dung.");
                    return;
                }

                byte[] data = Encoding.UTF8.GetBytes(message);

                udpClient.Send(data, data.Length, ipAddress, port);

                // MessageBox.Show("Message gửi thành công.");
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Port chưa đúng.");
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Socket error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btn_clear(object sender, EventArgs e)
        {
            txtMessage.Clear();
        }
    }
}
