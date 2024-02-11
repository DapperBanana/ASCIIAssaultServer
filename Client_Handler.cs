using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class ClientHandler
    {
        private TcpClient tcpClient;
        private Server server;
        private NetworkStream clientStream;
        private string? clientName;

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.server = server;
            clientStream = tcpClient.GetStream();
        }

        public void HandleClient()
        {
            Console.WriteLine("Client connected: " + tcpClient.Client.RemoteEndPoint);


            //Attempt login
            if (!TryLogin())
            {
                // If login fails, disconnect the client
                Console.WriteLine("Login failed. Disconnecting client: " + tcpClient.Client.RemoteEndPoint);
                tcpClient.Close();
                server.RemoveClient(this);
                return;
            }


            clientName = "Client" + Guid.NewGuid().ToString().Substring(0, 5);

            SendMessage("Welcome to the chat room, " + clientName + "!");

            while (true)
            {
                try
                {
                    byte[] messageBytes = new byte[4096];
                    int bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length);
                    if (bytesRead == 0)
                        break;

                    string message = Encoding.ASCII.GetString(messageBytes, 0, bytesRead);

                    // Check for the "logout" command
                    if (message.ToLower() == "logout")
                    {
                        HandleLogout();
                        break; // Exit the loop after handling logout
                    }

                    server.BroadcastMessage(clientName + ": " + message, this);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    break;
                }
            }

            Console.WriteLine("Client disconnected: " + clientName);
            server.BroadcastMessage(clientName + " has left the chat.", this);
            tcpClient.Close();
            server.RemoveClient(this);
        }

        public void SendMessage(string message)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            clientStream.Write(messageBytes, 0, messageBytes.Length);
            clientStream.Flush();
        }

        private bool TryLogin()
        {
            
            SendMessage("Please enter your username:");
            string username = WaitForClientResponse();

            SendMessage("Please enter your password:");
            string password = WaitForClientResponse();

            // Check if the provided credentials are valid (replace with your own validation logic)
            if (SQL_Handler.verifyUser(username, password))
            {
                SendMessage("Login successful. Welcome, " + username + "!");
                return true;
            }
            else
            {
                SendMessage("Invalid credentials. Please try again.");
                return false;
            }
        }

        private string WaitForClientResponse()
        {
            byte[] messageBytes = new byte[4096];
            int bytesRead = clientStream.Read(messageBytes, 0, messageBytes.Length);
            return Encoding.ASCII.GetString(messageBytes, 0, bytesRead);
        }

        private bool IsValidCredentials(string username, string password)
        {
            // Replace this with your own logic to validate credentials
            // For simplicity, using hardcoded values here
            return username == "user" && password == "pass";
        }

        private void HandleLogout()
        {
            SendMessage("Logout successful. Goodbye!");
            tcpClient.Close();
            server.RemoveClient(this);
        }
    }
}
