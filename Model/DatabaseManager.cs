using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace Freelance_Finance_Manager
{
    public static class DatabaseManager
    {
        private static readonly string server = "localhost";
        private static readonly string user = "root";
        private static readonly string password = "admin123";
        private static readonly string database = "freelance_finance_db";

        private static readonly string connectionString = $"Server={server};Uid={user};Pwd={password};";

        public static void InitializeDatabase()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {database};", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                CreateTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Initialisierung der Datenbank: {ex.Message}");
                throw;
            }
        }

        private static void CreateTables()
        {
            string dbConnectionString = $"Server={server};Database={database};Uid={user};Pwd={password};";

            using (var connection = new MySqlConnection(dbConnectionString))
            {
                connection.Open();

                var commands = new string[]
                {
                    "CREATE TABLE IF NOT EXISTS User (UserID INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(100), Email VARCHAR(100), PasswordHash VARCHAR(255));",
                    "CREATE TABLE IF NOT EXISTS Category (CategoryID INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(100), Type ENUM('Income', 'Expense'));",
                    "CREATE TABLE IF NOT EXISTS Income (IncomeID INT AUTO_INCREMENT PRIMARY KEY, UserID INT, Amount DECIMAL(10,2), Date DATE, CategoryID INT, Description TEXT, FOREIGN KEY(UserID) REFERENCES User(UserID), FOREIGN KEY(CategoryID) REFERENCES Category(CategoryID));",
                    "CREATE TABLE IF NOT EXISTS Expense (ExpenseID INT AUTO_INCREMENT PRIMARY KEY, UserID INT, Amount DECIMAL(10,2), Date DATE, CategoryID INT, Description TEXT, FOREIGN KEY(UserID) REFERENCES User(UserID), FOREIGN KEY(CategoryID) REFERENCES Category(CategoryID));",
                    "CREATE TABLE IF NOT EXISTS TaxEstimate (TaxID INT AUTO_INCREMENT PRIMARY KEY, UserID INT, Year INT, EstimatedTaxAmount DECIMAL(10,2), FOREIGN KEY(UserID) REFERENCES User(UserID));",
                    "CREATE TABLE IF NOT EXISTS BudgetForecast (ForecastID INT AUTO_INCREMENT PRIMARY KEY, UserID INT, Month INT, PlannedIncome DECIMAL(10,2), PlannedExpense DECIMAL(10,2), FOREIGN KEY(UserID) REFERENCES User(UserID));"
                };

                foreach (var cmdText in commands)
                {
                    using (var command = new MySqlCommand(cmdText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static MySqlConnection GetConnection()
        {
            var dbConnectionString = $"Server={server};Database={database};Uid={user};Pwd={password};";
            return new MySqlConnection(dbConnectionString);
        }

        public static void AddUser(string name, string email, string password)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    string hashedPassword = HashPassword(password);
                    string query = "INSERT INTO User (Name, Email, PasswordHash) VALUES (@Name, @Email, @PasswordHash);";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Benutzer erfolgreich hinzugefügt.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Hinzufügen des Benutzers: {ex.Message}");
                throw;
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
