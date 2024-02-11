using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;


class Server
{
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("config.json", optional: true, reloadOnChange: true)
        .Build();

    private TcpListener? tcpListener;
    private List<ClientHandler> clients = new List<ClientHandler>();

    public void StartServer()
    {
        tcpListener = new TcpListener(IPAddress.Any, 6969);
        tcpListener.Start();

        string connectionString = config.GetConnectionString("MyDatabase");

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

class ClientHandler
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
        if (IsValidCredentials(username, password))
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

class Game_Master
{
    static void Main()
    {
        Server server = new Server();
        server.StartServer();
    }
}
