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
using System.Threading;


namespace Pentapic
{
    public partial class Form1 : Form
    {
        //i entspricht x und k entspricht y
        //i equals to x and k equals to y
        Socket TCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string frame = "";
        int dia = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool v = false;
            MessageBox.Show("Verbindungsaufbau läuft...", "Verbindungstest");
            try
            {
                Connect("172.22.99.6", 1338);
                Send("0200000000ff",false);
            }
            catch
            {
                MessageBox.Show("Keine Verbindung!", "Fehler");
                v = true;
            }
            if (v == false)
            {
                MessageBox.Show("Verbindung erfolgreich wenn Pentawall Blau.", "Verbindung aufgebaut");
            }
        }
        private void Send(string HexData, bool Message)
        {
            HexData = HexData + "\r\n";
            Byte[] Data = Encoding.ASCII.GetBytes(HexData);
            Byte[] Rec = new Byte[2];
            TCP.Send(Data);
            TCP.Receive(Rec);
            string recs = Encoding.ASCII.GetString(Rec);
            while (recs != "ok")
            {
                TCP.Send(Data);
                TCP.Receive(Rec);
                recs = Encoding.ASCII.GetString(Rec);
            }
            if (Message)
            {
                MessageBox.Show("Übertragung Abgeschlossen");
            }
            //TCP.Shutdown(SocketShutdown.Both);
            //TCP.Close();
        }
        public void Connect(string IPAdr, int Port)
        {       
            IPAddress ipo = IPAddress.Parse(IPAdr);
            IPEndPoint ipEo = new IPEndPoint(ipo, Port);
            TCP.Connect(ipEo);
            timer2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Send(HexE.Text, true);

        }

        private void makeframe(string[,] Framedata)
        {
            
            frame = "03";
            for (int k = 0; k <= 14; k++)
            {
                for (int i = 0; i <= 15; i++)
                {
                    frame = frame + Framedata[i, k];
                }
            }
        }
        private string StringToHex(string hexstring)
        {
            var sb = new StringBuilder();
            foreach (char t in hexstring)
                sb.Append(Convert.ToInt32(t).ToString("x") + " ");
            return sb.ToString();
        }


        private void bsuch_Click(object sender, EventArgs e)
        {
            string picdata = "";
            openFileDialog1.ShowDialog();
            picdata=openFileDialog1.FileName;
            pictureBox1.Load(picdata);
            Bitmap img = new Bitmap(picdata);
            Color pix = new Color();
            string[,] pixrgb = new string [16,15];
            string[,] pixrgbhex = new String[16,15];
            for (int i = 0;i<=15;i++)
            {
                for (int k = 0; k <= 14; k++)
                {
                    try
                    {
                        pix = img.GetPixel(i, k);
                    }
                    catch { }
                    byte[] pixbit = { pix.R, pix.G, pix.B };
                    pixrgbhex[i, k] = BitConverter.ToString(pixbit).Replace("-", string.Empty);
                    /*pixargb = pix.
                    pixargbs = pixargb.ToString();
                    for(int j=2;j<=7;j++)
                    {
                        pixrgb[i, k] = pixrgb[i, k] + pixargbs[j];
                    }*/
                    
               }
            }
            makeframe(pixrgbhex);
                        
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Send(frame,true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button2.Enabled = false;
            Send("05",true);
            timer1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button2.Enabled = true;
            Send("06",false);
            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            taketime = taketime + timer1.Interval;
            if (taketime >= 295000)
            {
                button3_Click(sender, e);
            }
        }
        int taketime = 0;

        private void timer2_Tick(object sender, EventArgs e)
        {
            Send("01", false);
        }

        public void button4_Click(object sender, EventArgs e)
        {
            int i = 0;
            openFileDialog2.ShowDialog();
            int leng = openFileDialog2.FileNames.GetLength(0);
            string[] File = openFileDialog2.FileNames;
            //timer3.Enabled = true;
            for (i = 1; i <= leng; i++)
            {
                while (dia < i)
                {
                    Thread.Sleep(5000);
                    dia++;
                }
                Bitmap img = new Bitmap(File[i - 1]);
                Color pix = new Color();
                string[,] pixrgb = new string[16, 15];
                string[,] pixrgbhex = new String[16, 15];
                for (int k = 0; k <= 15; k++)
                {
                    for (int j = 0; j <= 14; j++)
                    {
                        try
                        {
                            pix = img.GetPixel(k, j);
                        }
                        catch { }
                        byte[] pixbit = { pix.R, pix.G, pix.B };
                        pixrgbhex[k, j] = BitConverter.ToString(pixbit).Replace("-", string.Empty);

                    }
                }
                makeframe(pixrgbhex);
                Send(frame, false);
            }
            dia = 1;

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            dia++;
        }

        private void activateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HexE.Enabled = true;
            bsend.Enabled = true;
        }
        
    }

}
