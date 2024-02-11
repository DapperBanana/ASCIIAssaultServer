using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class Server
    {

        private TcpListener? tcpListener;
        private List<ClientHandler> clients = new List<ClientHandler>();

        public void StartServer()
        {

            tcpListener = new TcpListener(IPAddress.Any, 6969);
            tcpListener.Start();




            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("New client connected.");

                ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                clients.Add(clientHandler);

                Thread clientThread = new Thread(new ThreadStart(clientHandler.HandleClient));
                clientThread.Start();
            }

        }

        public void RemoveClient(ClientHandler client)
        {
            clients.Remove(client);
        }

        public void BroadcastMessage(string message, ClientHandler sender)
        {
            foreach (var client in clients)
            {
                if (client != sender)
                {
                    client.SendMessage(message);
                }
            }
        }
    }
}
