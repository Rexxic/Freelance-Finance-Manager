using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Freelance_Finance_Manager
{
    // Model-Klasse für Expense
    public class Expense
    {
        public int ExpenseID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
    }

    // Repository-Klasse für Expense
    public class ExpenseRepository : BaseRepository<Expense>
    {
        public override void Add(Expense expense)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    INSERT INTO Expense (UserID, Amount, Date, CategoryID, Description)
                    VALUES (@UserID, @Amount, @Date, @CategoryID, @Description);
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", expense.UserID);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@CategoryID", expense.CategoryID);
                    command.Parameters.AddWithValue("@Description", expense.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override Expense GetById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Expense WHERE ExpenseID = @ExpenseID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExpenseID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Expense
                            {
                                ExpenseID = reader.GetInt32("ExpenseID"),
                                UserID = reader.GetInt32("UserID"),
                                Amount = reader.GetDecimal("Amount"),
                                Date = reader.GetDateTime("Date"),
                                CategoryID = reader.GetInt32("CategoryID"),
                                Description = reader.GetString("Description")
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public override List<Expense> GetAll()
        {
            var expenses = new List<Expense>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Expense;";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        expenses.Add(new Expense
                        {
                            ExpenseID = reader.GetInt32("ExpenseID"),
                            UserID = reader.GetInt32("UserID"),
                            Amount = reader.GetDecimal("Amount"),
                            Date = reader.GetDateTime("Date"),
                            CategoryID = reader.GetInt32("CategoryID"),
                            Description = reader.GetString("Description")
                        });
                    }
                }
            }
            return expenses;
        }

        public override void Update(Expense expense)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    UPDATE Expense
                    SET UserID      = @UserID,
                        Amount      = @Amount,
                        Date        = @Date,
                        CategoryID  = @CategoryID,
                        Description = @Description
                    WHERE ExpenseID = @ExpenseID;
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", expense.UserID);
                    command.Parameters.AddWithValue("@Amount", expense.Amount);
                    command.Parameters.AddWithValue("@Date", expense.Date);
                    command.Parameters.AddWithValue("@CategoryID", expense.CategoryID);
                    command.Parameters.AddWithValue("@Description", expense.Description);
                    command.Parameters.AddWithValue("@ExpenseID", expense.ExpenseID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM Expense WHERE ExpenseID = @ExpenseID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExpenseID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
