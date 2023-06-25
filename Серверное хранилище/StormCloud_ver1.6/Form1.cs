using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StormCloud_ver1._6
{
    public partial class Form1 : Form
    {
        string IP;
        int port = 8000;
        string fileNameMes;
        bool doStop = true;


        StreamReader sr;
        StreamWriter sw;

        List<string> dirList = new List<string>();
        ImageList imageList = new ImageList();
        TcpClient tcpClient;
        NetworkStream stream;
        Thread thread;

        public Form1()
        {
            InitializeComponent();
            ListImgn();
            textBox1.Text = "25.66.120.54";
        }
      
        void ListImgn()
        {
            imageList = new ImageList();

            imageList.ImageSize = new Size(70, 70);

            imageList.Images.Add(new Bitmap("icon/DOC.png"));
            imageList.Images.Add(new Bitmap("icon/JPG.png"));
            imageList.Images.Add(new Bitmap("icon/NON.png"));
            imageList.Images.Add(new Bitmap("icon/PDF.png"));
            imageList.Images.Add(new Bitmap("icon/PNG.png"));
            imageList.Images.Add(new Bitmap("icon/RAR.png"));
            imageList.Images.Add(new Bitmap("icon/TXT.png"));
            imageList.Images.Add(new Bitmap("icon/ZIP.png"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        async void ClientStart()
        {
            try
            {
                tcpClient = new TcpClient();
                //Console.WriteLine("Клиент запущен");
               
                 await tcpClient.ConnectAsync(IP, port);
                if (tcpClient.Connected)
                {
                    label2.Text = "Подключение установлено";
                    button3.Enabled = false;
                }
                //sr = new StreamReader(tcpClient.GetStream());
                sw = new StreamWriter(tcpClient.GetStream());
                sw.AutoFlush = true;
                sw.WriteLine($"c{Name}$");
                // if (tcpClient.Connected)
                //   Console.WriteLine($"Подключение с {tcpClient.Client.RemoteEndPoint} установлено");
                if (tcpClient.Connected)
                {
                    label2.Text = "Подключение установлено";
                    button3.Enabled = false;
                }

                sw.WriteLine("l");
                    ThreadStart readList = new ThreadStart(Mes);
                     thread = new Thread(readList);
                    thread.Start();
                
            }
            catch
            {
                label2.Text = "Подключение отсутсвует";
                MessageBox.Show("Возникла ошибка");
            }
        }

        void LoadIcon()
        {

            List<string> extension = new List<string>();
            string a;

            for (int i = 0; i < dirList.Count; i++)
            {
                a = "/";
                for (int j = 0; j < 3; j++)
                {
                    a += dirList[i][dirList[i].Count() - (3 - j)];
                }

                extension.Add(a);

            }
            Invoke((MethodInvoker)delegate {
                listView1.Items.Clear();
                listView1.SmallImageList = imageList;
            for (int i = 0; i < dirList.Count; i++)
            {
                if (dirList[i] != "")
                {
                    ListViewItem listViewItem = new ListViewItem(new string[] { "", dirList[i], "" });
                    switch (extension[i])
                    {
                        case "/txt":
                            listViewItem.ImageIndex = 6;
                            break;
                        case "/doc":
                            listViewItem.ImageIndex = 0;
                            break;
                        case "/ocx":
                            listViewItem.ImageIndex = 0;
                            break;
                        case "/jpg":
                            listViewItem.ImageIndex = 1;
                            break;
                        case "/pdf":
                            listViewItem.ImageIndex = 3;
                            break;
                        case "/png":
                            listViewItem.ImageIndex = 4;
                            break;
                        case "/rar":
                            listViewItem.ImageIndex = 5;
                            break;
                        case "/zip":
                            listViewItem.ImageIndex = 7;
                            break;
                        default:
                            listViewItem.ImageIndex = 2;
                            break;

                    }
                    listView1.Items.Add(listViewItem);
                }
                }
            });

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tcpClient!=null)Disconect();
            Close();
        }

        void Disconect() {
            if (tcpClient.Connected)
            {
                sw.Write("d");
                sw.Close();
                stream.Close();
                tcpClient.Close();
                thread.Abort();
                Application.Exit();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Подключиться
            if ((textBox1.Text == "") || (textBox2.Text == "")) { MessageBox.Show("Ошибка! Повторите попытку"); }
            else
            {
                IP = textBox1.Text;
                Name = textBox2.Text;
                ClientStart();
            }

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            //Загрузть
            try { 
            var fileContent = string.Empty;
            var filePath = string.Empty;
            filePath = openFileDialog1.FileName;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string[] filename = openFileDialog1.FileName.Split('\\');
                    //var fileStream = openFileDialog1.OpenFile();
                    var file = File.OpenRead(openFileDialog1.FileName);
                    var length = file.Length;
                    byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
                    sw.Write("f" + filename[filename.Length - 1] + "$");
                    await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
                    await file.CopyToAsync(stream);
                }
            }
            catch
            {
                MessageBox.Show("Файл поврежден");
            }
        }



        void Mes()
        {
         
            stream = tcpClient.GetStream();
            int a=8;
            while (doStop==true)
            {
                try
                {
                    if (tcpClient?.Connected == true)
                    {
                        a = stream.ReadByte();
                        switch (a) {
                            case 'l':
                                {
                                    dirList.Clear();
                                    var response = new List<byte>();
                                    var responseData = new byte[512];

                                    int bytes;  // количество полученных байтов
                                    while ((a = stream.ReadByte()) != '$')
                                    {
                                        // добавляем в буфер
                                        response.Add((byte)a);
                                    }
                                    var translation = Encoding.UTF8.GetString(response.ToArray());
                                    string[] beetwList;
                                    beetwList = translation.Split('C', 'O', 'N');
                                    for (int i = 0; i < beetwList.Length; i++)
                                    {
                                        if (beetwList[i] != "") { dirList.Add(beetwList[i]); }
                                    }
                                    response.Clear();
                                    LoadIcon();
                                }break;
                            case 'c': {
                                    Invoke((MethodInvoker)delegate {
                                        var response = new List<byte>();
                                    while ((a = stream.ReadByte()) != '$')
                                    {
                                        // добавляем в буфер
                                        response.Add((byte)a);
                                    }
                                    var translation = Encoding.UTF8.GetString(response.ToArray());
                                    string[] listname = translation.Split(' ');
                                    
                                        listView2.Items.Clear();
                                        for (int k = 0; k < listname.Length; k++)
                                        {
                                            if(listname[k]!="")
                                            listView2.Items.Add(listname[k]);
                                        }
                                    });
                                    }
                                break;
                            case 's': {
                                    Invoke((MethodInvoker)delegate {
                                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                                    saveFileDialog.FileName += fileNameMes;
                                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                    {

                                        var file = File.Create(saveFileDialog.FileName);
                                        ReadBytes(sizeof(long));
                                        long remainingLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));
                                        while (remainingLength > 0)
                                        {
                                            int lengthToRead = (int)Math.Min(remainingLength, buf.Length);
                                            ReadBytes(lengthToRead);
                                            file.Write(buf, 0, lengthToRead);
                                            remainingLength -= lengthToRead;
                                        }
                                        file.Close();

                                    }
                                    });
                                }
                                break;
                    }
                    } 
                    Task.Delay(1000).Wait();
                }
                catch (Exception e)
                {

                }
            }
        }

       
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        byte[] buf = new byte[65536];

        void ReadBytes(int howmuch)
        {
            int readPos = 0;
            while (readPos < howmuch)
            {
                var actuallyRead = stream.Read(buf, readPos, howmuch - readPos);
                if (actuallyRead == 0)
                    throw new EndOfStreamException();
                readPos += actuallyRead;
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //удалить
            var namefile = listView1.SelectedItems;
            if (namefile.Count != 0)
            {
                var fileNameMes = namefile[0].SubItems[1].Text;
                sw.Write("x" + fileNameMes + "$");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var namefile = listView1.SelectedItems;
            if (namefile.Count != 0)
            {
                 fileNameMes = namefile[0].SubItems[1].Text;
                sw.Write("s" + fileNameMes + "$");
            }
        }
    }
    
}
