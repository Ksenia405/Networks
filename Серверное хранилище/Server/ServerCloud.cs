using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Management;

namespace Server
{
    class ServerCloud
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 8000);
        TcpClient tcpClient;
        List<ConnectedClient> clients = new List<ConnectedClient>();
        string[] files;
        NetworkStream stream;
        string name;
        Thread thread;

        public async Task<int> StartServer()
        {
            try
            {
                tcpListener.Start();    // запускаем сервер
                Console.WriteLine($"Сервер запущен. Ожидание подключений... {IPAddress.Any}");
                Console.WriteLine(tcpListener.LocalEndpoint);
                while (true)
                {
                    // получаем подключение в виде TcpClient
                    tcpClient = await tcpListener.AcceptTcpClientAsync();
                    
                    Console.WriteLine($"Входящее подключение: {tcpClient.Client.RemoteEndPoint}");

                    
                        ThreadStart readList = new ThreadStart(ReadClient);
                        thread = new Thread(readList);
                        thread.Start();

                    info_for_server.info_for_server.Pr();

                }
            }
            finally
            {
                tcpListener.Stop(); // останавливаем сервер
            }
            return 1;

        }
 
        byte[] buf = new byte[65536];
        byte[] b = new byte[1024];
         void  ReadClient()
        {
            stream = tcpClient.GetStream();
            int a;
            
            while (true) {

                a = stream.ReadByte();
                        switch (a)
                        {
                            case 'c':
                                {

                                    var response = new List<byte>();
                            
                             while ((a = stream.ReadByte()) != '$')
                             {
                                 // добавляем в буфер
                                 response.Add((byte)a);
                             }
                            
                                    var translation = Encoding.UTF8.GetString(response.ToArray());
                                    clients.Add(new ConnectedClient(tcpClient, translation));
                                    name = translation;
                                    SendAllClients();
                                   
                                }
                                break;
                            case 'l':
                                {

                                   SendList(tcpClient);
                                    
                                }
                                break;
                            case 's':
                                {
                                 var response = new List<byte>();
                                while ((a = stream.ReadByte()) != '$')
                                {
                                 // добавляем в буфер
                                 response.Add((byte)a);
                                }
                                 var translation = Encoding.UTF8.GetString(response.ToArray());
                                  SendF(translation);
                                }
                                break;
                            case 'x':
                                {
                                    var response = new List<byte>();
                                    while ((a = stream.ReadByte()) != '$')
                                    {
                                        // добавляем в буфер
                                        response.Add((byte)a);
                                    }
                                    var translation = Encoding.UTF8.GetString(response.ToArray());
                                    Del(translation);
                                    
                                }
                                break;
                            case 'f':
                               {
                                    var response = new List<byte>();
                                    while ((a = stream.ReadByte()) != '$')
                                    {
                                        // добавляем в буфер
                                        response.Add((byte)a);
                                    }
                                    var translation = Encoding.UTF8.GetString(response.ToArray());
                                    var file = File.Create("Cloud/" + translation);

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
                            info_for_server.info_for_server.Check();
                            SendAllClientsList();

                                }
                                break;
                            case 'd':
                                {
                                    for (int j = 0; j < clients.Count; j++)
                                    {
                                        if (clients[j].Client == tcpClient)
                                        {
                                            clients.Remove(clients[j]);
                                            break;
                                        }
                                    }
                                    SendAllClients();
                                }
                                break;
                        
                }
            }

       
        }
        void ReadBytes(int howmuch)
        {
            int readPos = 0;
            while (readPos < howmuch)
            {
                var actuallyRead =  stream.Read(buf, readPos, howmuch - readPos);
                if (actuallyRead == 0)
                    throw new EndOfStreamException();
                readPos += actuallyRead;
            }
        }
        void Del(string translation)
        {
            File.Delete("Cloud/" + translation);
            SendAllClientsList();
        }
        void SendF(string translation) {
            try
            {
                var file = File.OpenRead("Cloud/" + translation);
                var length = file.Length;
                byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));

                byte s = Convert.ToByte('s');
                byte[] d = { s };
                stream.Write(d, 0, 1);
                stream.Write(lengthBytes, 0, lengthBytes.Length);
                file.CopyTo(stream);
                file.Close();
            }
            catch
            {
                byte[] lengthBytes = Encoding.UTF8.GetBytes(("Error"));
                byte s = Convert.ToByte('s');
                byte[] d = { s };
                stream.Write(d, 0, 1);
                stream.Write(lengthBytes, 0, lengthBytes.Length);
            }
        }


        void SendAllClients()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Client.Connected)
                {
                    var sw = new StreamWriter(clients[i].Client.GetStream());
                    sw.AutoFlush = true;
                    sw.Write("c");
                    for (int j = 0; j < clients.Count; j++)
                        sw.Write(clients[j].Name + " ");
                    sw.Write("$");
                }
            }
        }

        void SendAllClientsList()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Client.Connected)
                {
                    SendList(clients[i].Client);
                }
            }
        }
        void SendList(TcpClient tcpClient)
        {
           
            try{
                var stream = tcpClient.GetStream();
                string dirName = "Cloud/";
                // если папка существует
                if (Directory.Exists(dirName))
                {
                    //Console.WriteLine("Подкаталоги:");
                    string[] dirs = Directory.GetDirectories(dirName);
                    foreach (string s in dirs)
                    {
                        Console.WriteLine(s);
                    }
                    Console.WriteLine();
                    //Console.WriteLine("Файлы:");
                    files = Directory.GetFiles(dirName);

                    for (int i = 0; i < files.Length; i++)
                    {
                        files[i] = files[i].Remove(0, 6);
                        files[i] += "CON";
                    }
                    files[files.Length - 1] += "$";
                }
                 stream.Write(Encoding.UTF8.GetBytes("l"), 0, 1);
                for (int i = 0; i < files.Length; i++)
                {
                     stream.Write(Encoding.UTF8.GetBytes(files[i]), 0, Encoding.UTF8.GetBytes(files[i]).Length);
                }
                 stream.Write(Encoding.UTF8.GetBytes("$"), 0, 1);
            }
            catch 
            {
                
            }
        }

    
}
}
