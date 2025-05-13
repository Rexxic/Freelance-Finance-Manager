using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Freelance_Finance_Manager;
using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System.Reflection;
using Mysqlx.Crud;

namespace UnitTest_Projekt
{
    [TestClass]
    public class BudgetForecastRepositoryTests
    {
        private BudgetForecastRepository _forecastRepo;
        private UserRepository _userRepo;
        private int _testUserId;

        [TestInitialize]
        public void Setup()
        {
            // Datenbank initialisieren und Tabellen anlegen
            DatabaseManager.InitializeDatabase();

            _forecastRepo = new BudgetForecastRepository();
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

            // BudgetForecast-Tabelle bereinigen
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM BudgetForecast;", conn))
                    cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Add_Should_Insert_BudgetForecast_Record()
        {
            var forecast = new BudgetForecast
            {
                UserID = _testUserId,
                Month = 5,
                PlannedIncome = 1000m,
                PlannedExpense = 500m
            };
            _forecastRepo.Add(forecast);
            var all = _forecastRepo.GetAll();
            Assert.IsTrue(all.Any(f => f.Month == 5
                                     && f.PlannedIncome == 1000m
                                     && f.PlannedExpense == 500m));
        }

        [TestMethod]
        public void GetById_Should_Return_Correct_BudgetForecast()
        {
            var forecast = new BudgetForecast
            {
                UserID = _testUserId,
                Month = 6,
                PlannedIncome = 2000m,
                PlannedExpense = 800m
            };
            _forecastRepo.Add(forecast);
            var inserted = _forecastRepo.GetAll().Last();

            var fetched = _forecastRepo.GetById(inserted.ForecastID);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(6, fetched.Month);
            Assert.AreEqual(2000m, fetched.PlannedIncome);
            Assert.AreEqual(800m, fetched.PlannedExpense);
        }

        [TestMethod]
        public void GetAll_Should_Return_List_Of_BudgetForecasts()
        {
            _forecastRepo.Add(new BudgetForecast { UserID = _testUserId, Month = 7, PlannedIncome = 300m, PlannedExpense = 100m });
            _forecastRepo.Add(new BudgetForecast { UserID = _testUserId, Month = 8, PlannedIncome = 400m, PlannedExpense = 200m });

            var list = _forecastRepo.GetAll();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Update_Should_Modify_Existing_BudgetForecast()
        {
            var forecast = new BudgetForecast { UserID = _testUserId, Month = 9, PlannedIncome = 500m, PlannedExpense = 250m };
            _forecastRepo.Add(forecast);
            var inserted = _forecastRepo.GetAll().Last();

            inserted.PlannedIncome = 600m;
            inserted.PlannedExpense = 300m;
            _forecastRepo.Update(inserted);

            var updated = _forecastRepo.GetById(inserted.ForecastID);
            Assert.AreEqual(600m, updated.PlannedIncome);
            Assert.AreEqual(300m, updated.PlannedExpense);
        }

        [TestMethod]
        public void Delete_Should_Remove_BudgetForecast_Record()
        {
            var forecast = new BudgetForecast { UserID = _testUserId, Month = 10, PlannedIncome = 700m, PlannedExpense = 350m };
            _forecastRepo.Add(forecast);
            var inserted = _forecastRepo.GetAll().Last();

            _forecastRepo.Delete(inserted.ForecastID);

            var fetched = _forecastRepo.GetById(inserted.ForecastID);
            Assert.IsNull(fetched);
        }
    }
}