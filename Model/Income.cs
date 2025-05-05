using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Freelance_Finance_Manager
{
    // Model-Klasse für Income
    public class Income
    {
        public int IncomeID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
    }

    // Repository-Klasse für Income
    public class IncomeRepository : BaseRepository<Income>
    {
        public override void Add(Income income)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    INSERT INTO Income (UserID, Amount, Date, CategoryID, Description)
                    VALUES (@UserID, @Amount, @Date, @CategoryID, @Description);
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", income.UserID);
                    command.Parameters.AddWithValue("@Amount", income.Amount);
                    command.Parameters.AddWithValue("@Date", income.Date);
                    command.Parameters.AddWithValue("@CategoryID", income.CategoryID);
                    command.Parameters.AddWithValue("@Description", income.Description);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override Income GetById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Income WHERE IncomeID = @IncomeID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IncomeID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Income
                            {
                                IncomeID = reader.GetInt32("IncomeID"),
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

        public override List<Income> GetAll()
        {
            var incomes = new List<Income>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Income;";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        incomes.Add(new Income
                        {
                            IncomeID = reader.GetInt32("IncomeID"),
                            UserID = reader.GetInt32("UserID"),
                            Amount = reader.GetDecimal("Amount"),
                            Date = reader.GetDateTime("Date"),
                            CategoryID = reader.GetInt32("CategoryID"),
                            Description = reader.GetString("Description")
                        });
                    }
                }
            }
            return incomes;
        }

        public override void Update(Income income)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    UPDATE Income
                    SET UserID = @UserID,
                        Amount = @Amount,
                        Date   = @Date,
                        CategoryID  = @CategoryID,
                        Description = @Description
                    WHERE IncomeID = @IncomeID;
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", income.UserID);
                    command.Parameters.AddWithValue("@Amount", income.Amount);
                    command.Parameters.AddWithValue("@Date", income.Date);
                    command.Parameters.AddWithValue("@CategoryID", income.CategoryID);
                    command.Parameters.AddWithValue("@Description", income.Description);
                    command.Parameters.AddWithValue("@IncomeID", income.IncomeID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM Income WHERE IncomeID = @IncomeID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IncomeID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
