using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Freelance_Finance_Manager
{
    // Model-Klasse für TaxEstimate
    public class TaxEstimate
    {
        public int TaxID { get; set; }
        public int UserID { get; set; }
        public int Year { get; set; }
        public decimal EstimatedTaxAmount { get; set; }
    }

    // Repository-Klasse für TaxEstimate
    public class TaxEstimateRepository : BaseRepository<TaxEstimate>
    {
        public override void Add(TaxEstimate tax)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    INSERT INTO TaxEstimate (UserID, Year, EstimatedTaxAmount)
                    VALUES (@UserID, @Year, @EstimatedTaxAmount);
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", tax.UserID);
                    command.Parameters.AddWithValue("@Year", tax.Year);
                    command.Parameters.AddWithValue("@EstimatedTaxAmount", tax.EstimatedTaxAmount);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override TaxEstimate GetById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM TaxEstimate WHERE TaxID = @TaxID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TaxID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TaxEstimate
                            {
                                TaxID = reader.GetInt32("TaxID"),
                                UserID = reader.GetInt32("UserID"),
                                Year = reader.GetInt32("Year"),
                                EstimatedTaxAmount = reader.GetDecimal("EstimatedTaxAmount")
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public override List<TaxEstimate> GetAll()
        {
            var list = new List<TaxEstimate>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM TaxEstimate;";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TaxEstimate
                        {
                            TaxID = reader.GetInt32("TaxID"),
                            UserID = reader.GetInt32("UserID"),
                            Year = reader.GetInt32("Year"),
                            EstimatedTaxAmount = reader.GetDecimal("EstimatedTaxAmount")
                        });
                    }
                }
            }
            return list;
        }

        public override void Update(TaxEstimate tax)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = @"
                    UPDATE TaxEstimate
                    SET UserID             = @UserID,
                        Year               = @Year,
                        EstimatedTaxAmount = @EstimatedTaxAmount
                    WHERE TaxID = @TaxID;
                ";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", tax.UserID);
                    command.Parameters.AddWithValue("@Year", tax.Year);
                    command.Parameters.AddWithValue("@EstimatedTaxAmount", tax.EstimatedTaxAmount);
                    command.Parameters.AddWithValue("@TaxID", tax.TaxID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM TaxEstimate WHERE TaxID = @TaxID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TaxID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
