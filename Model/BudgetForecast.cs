using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Freelance_Finance_Manager
{
    // Model-Klasse für BudgetForecast
    public class BudgetForecast
    {
        public int ForecastID { get; set; }
        public int UserID { get; set; }
        public int Month { get; set; }
        public decimal PlannedIncome { get; set; }
        public decimal PlannedExpense { get; set; }
    }

    // Repository-Klasse für BudgetForecast
    public class BudgetForecastRepository : BaseRepository<BudgetForecast>
    {
        public override void Add(BudgetForecast forecast)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    INSERT INTO BudgetForecast (UserID, Month, PlannedIncome, PlannedExpense)
                    VALUES (@UserID, @Month, @PlannedIncome, @PlannedExpense);
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", forecast.UserID);
                    command.Parameters.AddWithValue("@Month", forecast.Month);
                    command.Parameters.AddWithValue("@PlannedIncome", forecast.PlannedIncome);
                    command.Parameters.AddWithValue("@PlannedExpense", forecast.PlannedExpense);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override BudgetForecast GetById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM BudgetForecast WHERE ForecastID = @ForecastID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ForecastID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BudgetForecast
                            {
                                ForecastID = reader.GetInt32("ForecastID"),
                                UserID = reader.GetInt32("UserID"),
                                Month = reader.GetInt32("Month"),
                                PlannedIncome = reader.GetDecimal("PlannedIncome"),
                                PlannedExpense = reader.GetDecimal("PlannedExpense")
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public override List<BudgetForecast> GetAll()
        {
            var list = new List<BudgetForecast>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM BudgetForecast;";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BudgetForecast
                        {
                            ForecastID = reader.GetInt32("ForecastID"),
                            UserID = reader.GetInt32("UserID"),
                            Month = reader.GetInt32("Month"),
                            PlannedIncome = reader.GetDecimal("PlannedIncome"),
                            PlannedExpense = reader.GetDecimal("PlannedExpense")
                        });
                    }
                }
            }
            return list;
        }

        public override void Update(BudgetForecast forecast)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    UPDATE BudgetForecast
                    SET UserID         = @UserID,
                        Month          = @Month,
                        PlannedIncome  = @PlannedIncome,
                        PlannedExpense = @PlannedExpense
                    WHERE ForecastID = @ForecastID;
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", forecast.UserID);
                    command.Parameters.AddWithValue("@Month", forecast.Month);
                    command.Parameters.AddWithValue("@PlannedIncome", forecast.PlannedIncome);
                    command.Parameters.AddWithValue("@PlannedExpense", forecast.PlannedExpense);
                    command.Parameters.AddWithValue("@ForecastID", forecast.ForecastID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM BudgetForecast WHERE ForecastID = @ForecastID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ForecastID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
