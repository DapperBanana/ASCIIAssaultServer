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
        private readonly object clientLock = new object();

        public void StartServer()
        {
            tcpListener = new TcpListener(IPAddress.Any, 6969);
            tcpListener.Start();

            Console.WriteLine("Server started on port 6969. Waiting for connections...");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Console.WriteLine("New connection received.");

                ClientHandler handler = new ClientHandler(tcpClient, this);
                lock (clientLock)
                {
                    clients.Add(handler);
                }

                Thread clientThread = new Thread(handler.HandleClient);
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }

        public void BroadcastMessage(string message, ClientHandler sender)
        {
            lock (clientLock)
            {
                foreach (ClientHandler client in clients)
                {
                    if (client != sender)
                    {
                        try
                        {
                            client.SendMessage(message);
                        }
                        catch
                        {
                            // client probably disconnected, will be cleaned up
                        }
                    }
                }
            }
        }

        public void RemoveClient(ClientHandler handler)
        {
            lock (clientLock)
            {
                clients.Remove(handler);
            }
            Console.WriteLine($"Client removed. Active connections: {clients.Count}");
        }
    }
}
