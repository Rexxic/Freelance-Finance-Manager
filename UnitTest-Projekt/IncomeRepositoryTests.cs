using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Freelance_Finance_Manager;

namespace UnitTest_Projekt
{
    [TestClass]
    public class IncomeRepositoryTests
    {
        private IncomeRepository _incomeRepo;
        private UserRepository _userRepo;
        private CategoryRepository _categoryRepo;
        private int _testUserId;
        private int _testCategoryId;

        [TestInitialize]
        public void Setup()
        {
            // Datenbank initialisieren und Tabellen anlegen
            DatabaseManager.InitializeDatabase();

            // Repository-Instanzen erzeugen
            _incomeRepo = new IncomeRepository();
            _userRepo = new UserRepository();
            _categoryRepo = new CategoryRepository();

            // Test-User anlegen
            var testUser = new User
            {
                Name = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash"
            };
            _userRepo.Add(testUser);
            _testUserId = _userRepo.GetAll().Last().UserID;

            // Test-Kategorie anlegen
            var testCategory = new Category
            {
                Name = "TestIncome",
                Type = CategoryType.Income
            };
            _categoryRepo.Add(testCategory);
            _testCategoryId = _categoryRepo.GetAll().Last().CategoryID;

            // Income-Tabelle bereinigen
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM Income;", conn))
                    cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Add_Should_Insert_Income_Record()
        {
            var income = new Income
            {
                UserID = _testUserId,
                Amount = 100m,
                Date = DateTime.Today,
                CategoryID = _testCategoryId,
                Description = "Test Einkommen"
            };
            _incomeRepo.Add(income);

            var all = _incomeRepo.GetAll();
            Assert.IsTrue(all.Any(i => i.Amount == 100m && i.Description == "Test Einkommen"));
        }

        [TestMethod]
        public void GetById_Should_Return_Correct_Income()
        {
            var income = new Income
            {
                UserID = _testUserId,
                Amount = 200m,
                Date = DateTime.Today,
                CategoryID = _testCategoryId,
                Description = "GetById Test"
            };
            _incomeRepo.Add(income);
            var inserted = _incomeRepo.GetAll().Last();

            var fetched = _incomeRepo.GetById(inserted.IncomeID);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(200m, fetched.Amount);
            Assert.AreEqual("GetById Test", fetched.Description);
        }

        [TestMethod]
        public void GetAll_Should_Return_List_Of_Incomes()
        {
            _incomeRepo.Add(new Income { UserID = _testUserId, Amount = 10m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "A" });
            _incomeRepo.Add(new Income { UserID = _testUserId, Amount = 20m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "B" });

            var list = _incomeRepo.GetAll();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Update_Should_Modify_Existing_Income()
        {
            var income = new Income { UserID = _testUserId, Amount = 50m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "Old" };
            _incomeRepo.Add(income);
            var inserted = _incomeRepo.GetAll().Last();

            inserted.Amount = 75m;
            inserted.Description = "Updated";
            _incomeRepo.Update(inserted);

            var updated = _incomeRepo.GetById(inserted.IncomeID);
            Assert.AreEqual(75m, updated.Amount);
            Assert.AreEqual("Updated", updated.Description);
        }

        [TestMethod]
        public void Delete_Should_Remove_Income_Record()
        {
            var income = new Income { UserID = _testUserId, Amount = 30m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "ToDelete" };
            _incomeRepo.Add(income);
            var inserted = _incomeRepo.GetAll().Last();

            _incomeRepo.Delete(inserted.IncomeID);

            var fetched = _incomeRepo.GetById(inserted.IncomeID);
            Assert.IsNull(fetched);
        }
    }
}
