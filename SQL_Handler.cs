using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace ASCIIAssault_Server
{
    public class SQL_Handler
    {
        public static string connectionString()
        {
            string? server = Utility.Config.GetConnectionString("Server");
            string? port = Utility.Config.GetConnectionString("Port");
            string? database = Utility.Config.GetConnectionString("Database");
            string? sa_username = Utility.Config.GetConnectionString("SA_username");
            string? sa_password = Utility.Config.GetConnectionString("SA_password");


            // Your connection string
            string connectionString = $"Server={server};Port={port};Database={database};User Id={sa_username};Password={sa_password};";
            return connectionString;
        }

        public static bool verifyUser(string username, string password)
        {
            bool return_val = false;

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
            {
                return return_val;
            }

            string? user_table = Utility.Config.GetConnectionString("User_Table");


            string ConnectionString = connectionString();
            string query = "SELECT username FROM " + user_table + " where password = @password and username = @username";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        //prevent SQL injection with a little parameterization
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@username", username);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Access data using reader
                                string userName = reader.GetString(reader.GetOrdinal("username"));
                                return_val = true;
                            }
                            else
                            {
                                Console.WriteLine("No records found in the user_table.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return return_val;
        }
    }
}
