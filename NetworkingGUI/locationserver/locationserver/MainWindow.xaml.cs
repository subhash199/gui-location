using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows;

namespace locationserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TcpListener listener;
            Socket connection;
            Handler requesthandler;
            listener = new TcpListener(IPAddress.Any, 43);
            listener.Start();
            Console.WriteLine("Server is Listening");
            while (true)
            {
                connection = listener.AcceptSocket();
                requesthandler = new Handler();
                Thread t = new Thread(() => requesthandler.clientRequest(connection));
                t.Start();
            }
        }
    class Handler
        {
            public Handler()
            {

            }
            public void clientRequest(Socket connection)
            {
                Dictionary<string, string> clients = new Dictionary<string, string>();
                NetworkStream socketStream = new NetworkStream(connection);
                Console.WriteLine("Connection Received");
                try
                {
                    bool html = false;
                    bool http1 = false;
                    bool http2 = false;
                    bool getPost = false;
                    string userName = "";
                    string location = "";


                    StreamWriter sw = new StreamWriter(socketStream);
                    StreamReader sr = new StreamReader(socketStream);

                    sw.AutoFlush = true;
                    socketStream.ReadTimeout = 1000;
                    socketStream.WriteTimeout = 1000;

                    string line = sr.ReadLine();

                    string[] splitLine = line.Split(' ');

                    if (line.StartsWith("PUT /"))
                    {
                        userName = sr.ReadLine();
                        location = sr.ReadLine();
                    }


                    if (line.StartsWith("GET /") || line.StartsWith("PUT /") || line.StartsWith("POST /"))
                    {
                        //html = true;
                        string[] httpSplit = line.Split(new char[] { ' ' });
                        userName = httpSplit[1];
                        if (line.Contains("HTTP/1.0"))
                        {
                            http1 = true;
                        }
                        else if (line.Contains("HTTP/1.1"))
                        {
                            http2 = true;
                        }
                        if (http1 == false && http2 == false)
                        {

                            if (line.StartsWith("GET /"))
                            {
                                //getPost = true;
                                html = true;
                                userName = userName.Remove(0, 1);
                                if (clients.ContainsKey(userName))
                                {

                                    sw.WriteLine("HTTP/0.9 200 OK\r\nContent-Type: text/plain\r\n" + clients[userName]);
                                }
                                else
                                {
                                    sw.WriteLine("HTTP/0.9 404 Not Found\r\nContent-Type: text/plain\r\n\r\n");
                                }
                            }
                            else if (line.StartsWith("PUT /"))
                            {
                                //getPost = true;
                                html = true;
                                userName = userName.Remove(0, 1);
                                if (clients.ContainsKey(userName))
                                {
                                    clients[userName] = clients[location];
                                    sw.WriteLine("HTTP/0.9 200 OK" + "\r\n" + "Content-Type: text/plain" + "\r\n\r\n");

                                }
                                else
                                {
                                    clients.Add(userName, location);
                                    sw.WriteLine("HTTP/0.9 200 OK\r\nContent-Type: text/plain\r\n\r\n");
                                }
                            }
                        }
                        else if (http1 == true)
                        {
                            if (line.StartsWith("GET /?"))
                            {
                                //getPost = true;
                                html = true;
                                userName = userName.Remove(0, 2);
                                if (clients.ContainsKey(userName))
                                {
                                    sw.WriteLine("HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n\r\n" + clients[userName] + "\r\n ");

                                }
                                else
                                {
                                    sw.WriteLine("HTTP/1.0 404 Not Found\r\nContent-Type: text/plain\r\n\r\n");

                                }
                            }
                            else if (line.StartsWith("POST /"))
                            {
                                //getPost = true;
                                html = true;
                                userName = userName.Remove(0, 2);
                                int count = httpSplit.Length;
                                for (int i = 0; i < httpSplit.Length; i++)
                                {
                                    if (i == count)
                                    {
                                        location += httpSplit[i];
                                    }
                                }

                                if (clients.ContainsKey(userName))
                                {
                                    clients[userName] = clients[location];
                                    sw.WriteLine("HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n\r\n");

                                }
                                else
                                {
                                    clients.Add(userName, location);
                                    sw.WriteLine("HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n\r\n");

                                }
                            }
                        }
                        else if (http2 == true)
                        {
                            if (line.StartsWith("GET /?"))
                            {
                                //getPost = true;
                                html = true;
                                userName = userName.Remove(0, 7);
                                if (clients.ContainsKey(userName))
                                {
                                    sw.WriteLine("HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n" + clients[userName] + "\r\n ");

                                }
                                else
                                {
                                    sw.WriteLine("HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\n");

                                }
                            }
                            else if (line.StartsWith("POST / HTTP/1.1"))
                            {
                                //getPost = true;
                                html = true;
                                sr.ReadLine();
                                sr.ReadLine();
                                sr.ReadLine();
                                string read = sr.ReadLine();
                                string tempName = "";
                                string[] usersplit = read.Split('=');
                                tempName = usersplit[1];
                                location = usersplit[2];

                                usersplit = tempName.Split('&');
                                userName = usersplit[0];

                                if (clients.ContainsKey(userName))
                                {
                                    clients[userName] = location;
                                    sw.WriteLine("HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n");

                                }
                                else
                                {
                                    clients.Add(userName, location);
                                    sw.WriteLine("OK");
                                }
                            }
                        }
                    }                    
                    if (html == false)
                    {
                        string[] splitLiness = line.Split(new char[] { ' ' }, 2);
                        if (line.Contains(" "))
                        {

                            if (clients.ContainsKey(splitLiness[0]))
                            {
                                clients[splitLine[0]] = splitLiness[1];  //mydictionary[mykey] = mynewvalue
                                sw.WriteLine("OK");
                            }
                            else
                            {
                                clients.Add(splitLiness[0], splitLiness[1]);
                                sw.WriteLine("OK");
                            }

                        }
                        else
                        {
                            if (clients.ContainsKey(line))
                            {
                                sw.WriteLine(clients[line]);
                            }

                            else
                            {
                                sw.WriteLine("ERROR: no entries found\r\n");

                            }

                        }
                    }

                    Console.WriteLine("Respond Received: " + line);


                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: " + ex);

                }
                finally
                {
                    socketStream.Close();
                    connection.Close();
                }


            }
        }
    }
}






