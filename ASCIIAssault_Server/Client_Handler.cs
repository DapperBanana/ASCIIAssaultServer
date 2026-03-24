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
        private bool authenticated = false;

        public ClientHandler(TcpClient tcpClient, Server server)
        {
            this.tcpClient = tcpClient;
            this.server = server;
            clientStream = tcpClient.GetStream();
        }

        public void HandleClient()
        {
            try
            {
                SendMessage("Welcome to ASCII Assault!");
                SendMessage("Type 'login' or 'register':");

                while (!authenticated && tcpClient.Connected)
                {
                    string? input = ReadMessage();
                    if (input == null) break;

                    input = input.Trim().ToLower();

                    if (input == "register")
                        HandleRegistration();
                    else if (input == "login")
                        HandleLogin();
                    else
                        SendMessage("Invalid option. Type 'login' or 'register':");
                }

                if (authenticated)
                {
                    SendMessage($"Welcome, {clientName}! You are now connected.");
                    server.BroadcastMessage($"{clientName} has joined.", this);
                    ListenForMessages();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        private void HandleRegistration()
        {
            SendMessage("Enter username:");
            string? username = ReadMessage()?.Trim();
            if (string.IsNullOrEmpty(username)) return;

            SendMessage("Enter password:");
            string? password = ReadMessage()?.Trim();
            if (string.IsNullOrEmpty(password)) return;

            if (SQL_Handler.RegisterUser(username, password))
            {
                SendMessage("Registration successful! You are now logged in.");
                clientName = username;
                authenticated = true;
            }
            else
            {
                SendMessage("Username already taken. Type 'login' or 'register':");
            }
        }

        private void HandleLogin()
        {
            SendMessage("Enter username:");
            string? username = ReadMessage()?.Trim();
            if (string.IsNullOrEmpty(username)) return;

            SendMessage("Enter password:");
            string? password = ReadMessage()?.Trim();
            if (string.IsNullOrEmpty(password)) return;

            if (SQL_Handler.AuthenticateUser(username, password))
            {
                clientName = username;
                authenticated = true;
            }
            else
            {
                SendMessage("Invalid credentials. Type 'login' or 'register':");
            }
        }

        private void ListenForMessages()
        {
            while (tcpClient.Connected)
            {
                string? message = ReadMessage();
                if (message == null) break;

                Console.WriteLine($"[{clientName}]: {message}");
                server.BroadcastMessage($"[{clientName}]: {message}", this);
            }
        }

        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            clientStream.Write(data, 0, data.Length);
        }

        private string? ReadMessage()
        {
            byte[] buffer = new byte[4096];
            int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0) return null;
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public string? GetClientName() => clientName;

        private void Disconnect()
        {
            if (clientName != null)
            {
                Console.WriteLine($"{clientName} disconnected.");
                server.BroadcastMessage($"{clientName} has left.", this);
            }
            server.RemoveClient(this);
            tcpClient.Close();
        }
    }
}
