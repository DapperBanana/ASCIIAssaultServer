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
            string? uid = Utility.Config.GetConnectionString("Uid");
            string? password = Utility.Config.GetConnectionString("Password");

            return $"Server={server};Port={port};Database={database};Uid={uid};Password={password};";
        }

        public static bool RegisterUser(string username, string password)
        {
            string hashedPassword = PasswordHelper.HashPassword(password);

            using (MySqlConnection conn = new MySqlConnection(connectionString()))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", username);
                    long count = (long)checkCmd.ExecuteScalar();
                    if (count > 0) return false;
                }

                string insertQuery = "INSERT INTO users (username, password_hash, created_at) VALUES (@username, @hash, @created)";
                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@hash", hashedPassword);
                    insertCmd.Parameters.AddWithValue("@created", DateTime.UtcNow);
                    insertCmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static bool AuthenticateUser(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString()))
            {
                conn.Open();

                string query = "SELECT password_hash FROM users WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object? result = cmd.ExecuteScalar();

                    if (result == null) return false;

                    string storedHash = result.ToString() ?? "";
                    return PasswordHelper.VerifyPassword(password, storedHash);
                }
            }
        }
    }
}
