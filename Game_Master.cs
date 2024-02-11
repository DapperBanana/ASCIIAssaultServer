using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;


namespace ASCIIAssault_Server
{
    class Game_Master
    {
        static void Main()
        {
            Server server = new Server();
            server.StartServer();
        }
    }
}