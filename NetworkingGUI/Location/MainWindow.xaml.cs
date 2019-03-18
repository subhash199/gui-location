using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Location
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void launch_button_Click_1(object sender, RoutedEventArgs e)
        {
            string host = host_button.Text;
            int port = int.Parse(port_button.Text);
            string userName = username_button.Text;
            string location = location_button.Text;
            string httpRequest = "";
            if (whois_button.IsChecked == true)
            {
                host = "whois.dcs.hull.ac.uk";
            }
            else if (h9_button.IsChecked == true)
            {
                httpRequest = "-h9";
            }
            else if (h0_button.IsChecked == true)
            {
                httpRequest = "-h0";
            }
            else if (h1_button.IsChecked == true)
            {
                httpRequest = "-h1";
            }
            string respond = "";
            int hostnum = port;


            bool http = false;

            bool h9 = false;
            bool h0 = false;
            bool h1 = false;

            if (httpRequest == "h9")
            {
                h9 = true;
            }
            else if (httpRequest == "h0")
            {
                h0 = true;
            }
            else if (httpRequest == "h1")
            {
                h1 = true;
            }

            TcpClient client = new TcpClient();
            try
            {

                client.Connect(host, hostnum);

                StreamWriter sw = new StreamWriter(client.GetStream());
                StreamReader sr = new StreamReader(client.GetStream());

                sw.AutoFlush = true;
                client.ReceiveTimeout = 1000;
                client.SendTimeout = 1000;

                if (h9 == true)
                {
                    http = true;


                    if (location == "")
                    {
                        sw.WriteLine("GET /" + userName + "\r\n");
                        string read = "";
                        for (int i = 0; i < 3; i++)
                        {
                            read = sr.ReadLine();
                        }
                        text_button.Text += userName + " is " + read;
                    }

                    else
                    {
                        sw.WriteLine("PUT /" + userName + "\r\n\r\n" + location + "\r\n");

                        if (sr.ReadLine().Contains("OK"))
                        {
                            text_button.Text += userName + " location changed to be " + location;
                        }

                    }


                }
                else if (h0 == true)
                {
                    http = true;
                    if (location == "")
                    {
                        sw.WriteLine("GET /?" + userName + " " + "HTTP/1.0\r\n\r\n");

                        string read = "";
                        for (int i = 0; i < 4; i++)
                        {
                            read = sr.ReadLine();
                        }

                        text_button.Text += userName + " is " + read;


                    }
                    else
                    {
                        sw.WriteLine("POST /" + userName + " HTTP/1.0\r\nContent-Length: " + location.Length + "\r\n\r\n" + location);

                        if (sr.ReadLine().Contains("OK"))
                        {
                            text_button.Text += userName + " location changed to be " + location;

                        }

                    }


                }
                else if (h1 == true)
                {
                    http = true;
                    if (location == "" && hostnum != 80)
                    {
                        sw.WriteLine("GET /?name=" + userName + " " + "HTTP/1.1\r\n" + "Host: " + host + "\r\n\r\n");
                        string line1 = sr.ReadLine();
                        string line2 = sr.ReadLine();
                        string line3 = sr.ReadLine();
                        string line4 = sr.ReadLine();

                        text_button.Text += userName + " is " + line4;
                    }
                    if (location == "" && hostnum == 80)
                    {
                        sw.WriteLine("GET /?name=" + userName + " " + "HTTP/1.1\r\n" + "Host: " + host + "\r\n\r\n");


                        List<string> htmlString = new List<string>();
                        while (sr.Peek() > -1)
                        {
                            respond = sr.ReadLine();
                            htmlString.Add(respond);
                        }
                        respond = "";
                        int spaceIndex = htmlString.IndexOf("");

                        for (int k = spaceIndex + 1; k < htmlString.Count; k++)
                        {
                            respond += htmlString[k];
                            respond += "\r\n";
                        }

                        text_button.Text += userName + " is " + respond;


                    }
                    else if (location != "")
                    {
                        int length = 15 + userName.Length + location.Length;
                        sw.WriteLine("POST / HTTP/1.1\r\nHost: " + host + "\r\nContent-Length: " + length + "\r\n\r\nname=" + userName + "&location=" + location);

                        if (sr.ReadLine().Contains("OK"))
                        {
                            text_button.Text += userName + " location changed to be " + location;
                        }
                        if (hostnum == 80)
                        {
                            while (sr.Peek() > -1)
                            {
                                respond += sr.ReadLine();
                            }
                            text_button.Text += respond;

                        }

                    }


                }


                else if (location == "" && http == false)
                {

                    sw.WriteLine(userName);
                    string reply = sr.ReadLine();

                    text_button.Text += userName + " is " + reply;
                }

                else if (location != "" && http == false)
                {
                    sw.WriteLine(userName + " " + location);

                    string reply = sr.ReadLine();
                    if (reply == "OK")
                    {
                        text_button.Text += userName + " location changed to be " + location;

                    }
                    else
                    {
                        text_button.Text += "Unexpected reply from server: " + reply;

                    }
                }
                else
                {
                    text_button.Text += "Too many arguments";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                client.Close();
            }

        }
    }
}





