using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freelance_Finance_Manager;
using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System.Reflection;

namespace UnitTest_Projekt
{
    [TestClass]
    public class TaxEstimateRepositoryTests
    {
        private TaxEstimateRepository _taxRepo;
        private UserRepository _userRepo;
        private int _testUserId;

        [TestInitialize]
        public void Setup()
        {
            // Datenbank initialisieren und Tabellen anlegen
            DatabaseManager.InitializeDatabase();

            _taxRepo = new TaxEstimateRepository();
            _userRepo = new UserRepository();

            // Test-User anlegen (FK erforderlich)
            var testUser = new User
            {
                Name = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash"
            };
            _userRepo.Add(testUser);
            _testUserId = _userRepo.GetAll().Last().UserID;

            // TaxEstimate-Tabelle bereinigen
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM TaxEstimate;", conn))
                    cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Add_Should_Insert_TaxEstimate_Record()
        {
            var tax = new TaxEstimate
            {
                UserID = _testUserId,
                Year = 2024,
                EstimatedTaxAmount = 1234.56m
            };
            _taxRepo.Add(tax);

            var all = _taxRepo.GetAll();
            Assert.IsTrue(all.Any(t => t.UserID == _testUserId && t.Year == 2024 && t.EstimatedTaxAmount == 1234.56m));
        }

        [TestMethod]
        public void GetById_Should_Return_Correct_TaxEstimate()
        {
            var tax = new TaxEstimate
            {
                UserID = _testUserId,
                Year = 2023,
                EstimatedTaxAmount = 2000m
            };
            _taxRepo.Add(tax);
            var inserted = _taxRepo.GetAll().Last();

            var fetched = _taxRepo.GetById(inserted.TaxID);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(2023, fetched.Year);
            Assert.AreEqual(2000m, fetched.EstimatedTaxAmount);
        }

        [TestMethod]
        public void GetAll_Should_Return_List_Of_TaxEstimates()
        {
            _taxRepo.Add(new TaxEstimate { UserID = _testUserId, Year = 2022, EstimatedTaxAmount = 100m });
            _taxRepo.Add(new TaxEstimate { UserID = _testUserId, Year = 2021, EstimatedTaxAmount = 200m });

            var list = _taxRepo.GetAll();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Update_Should_Modify_Existing_TaxEstimate()
        {
            var tax = new TaxEstimate { UserID = _testUserId, Year = 2025, EstimatedTaxAmount = 500m };
            _taxRepo.Add(tax);
            var inserted = _taxRepo.GetAll().Last();

            inserted.EstimatedTaxAmount = 750m;
            inserted.Year = 2026;
            _taxRepo.Update(inserted);

            var updated = _taxRepo.GetById(inserted.TaxID);
            Assert.AreEqual(750m, updated.EstimatedTaxAmount);
            Assert.AreEqual(2026, updated.Year);
        }

        [TestMethod]
        public void Delete_Should_Remove_TaxEstimate_Record()
        {
            var tax = new TaxEstimate { UserID = _testUserId, Year = 2020, EstimatedTaxAmount = 300m };
            _taxRepo.Add(tax);
            var inserted = _taxRepo.GetAll().Last();

            _taxRepo.Delete(inserted.TaxID);

            var fetched = _taxRepo.GetById(inserted.TaxID);
            Assert.IsNull(fetched);
        }
    }
}
