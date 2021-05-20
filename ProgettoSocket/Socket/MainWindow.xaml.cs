using System;
using System.Collections.Generic;
using System.Linq;
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
//Aggiunta delle seguenti librerie
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse("10.73.0.31"), 56000);
                       
            btnInvia.IsEnabled = true;

            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));

            ricezione.Start(sourceSocket);

            //btnCreaSocket serve a creare un socket con un ip e una porta impostati, crea anche un thread di ricezione con il socket ip, infine abilita btnInvia.
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //aggiungere controlli sul contenuto delle textbox
            string ipAddress = txtIP.Text;
            int port = int.Parse(txtPort.Text);

            SocketSend(IPAddress.Parse(ipAddress), port, txtMsg.Text);

            //btnInvia prende un ip e una porta e invia un messaggio al socket che ha i valori messi in precendenza
        }

        public async void SocketReceive(object socksource)
        {
            IPEndPoint ipendp = (IPEndPoint)socksource;

            Socket t = new Socket(ipendp.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            t.Bind(ipendp);

            Byte[] bytesRicevuti = new Byte[256];

            string message;

            int contaCaratteri = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    if(t.Available >0)
                    {
                        message = "";

                        contaCaratteri = t.Receive(bytesRicevuti, bytesRicevuti.Length,0);
                        message = message + Encoding.ASCII.GetString(bytesRicevuti, 0, contaCaratteri);

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            lblRicevi.Content = message;
                        }));

                    }

                }

            });
            //è il metodo usato dal thread in ricezione per ricevere i messaggi
        }

        public void SocketSend(IPAddress dest, int destport, string message)
        {
            Byte[] byteInviati = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destport);

            s.SendTo(byteInviati, remote_endpoint);

            //metodo usato per inviare i messaggi quando si usa btnInvia.
        }

    }
}
