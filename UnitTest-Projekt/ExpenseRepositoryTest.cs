using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Freelance_Finance_Manager;
using Freelance_Finance_Manager.Repositories;
using System.Reflection;

namespace UnitTest_Projekt
{
    [TestClass]
    public class ExpenseRepositoryTests
    {
        private ExpenseRepository _expenseRepo;
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
            _expenseRepo = new ExpenseRepository();
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

            // Test-Kategorie für Ausgaben anlegen
            var testCategory = new Category
            {
                Name = "TestExpense",
                Type = CategoryType.Expense
            };
            _categoryRepo.Add(testCategory);
            _testCategoryId = _categoryRepo.GetAll().Last().CategoryID;

            // Expense-Tabelle bereinigen
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM Expense;", conn))
                    cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Add_Should_Insert_Expense_Record()
        {
            var expense = new Expense
            {
                UserID = _testUserId,
                Amount = 123.45m,
                Date = DateTime.Today,
                CategoryID = _testCategoryId,
                Description = "Test Ausgabe"
            };
            _expenseRepo.Add(expense);

            var all = _expenseRepo.GetAll();
            Assert.IsTrue(all.Any(e => e.Amount == 123.45m && e.Description == "Test Ausgabe"));
        }

        [TestMethod]
        public void GetById_Should_Return_Correct_Expense()
        {
            var expense = new Expense
            {
                UserID = _testUserId,
                Amount = 200m,
                Date = DateTime.Today,
                CategoryID = _testCategoryId,
                Description = "GetById Test"
            };
            _expenseRepo.Add(expense);
            var inserted = _expenseRepo.GetAll().Last();

            var fetched = _expenseRepo.GetById(inserted.ExpenseID);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(200m, fetched.Amount);
            Assert.AreEqual("GetById Test", fetched.Description);
        }

        [TestMethod]
        public void GetAll_Should_Return_List_Of_Expenses()
        {
            _expenseRepo.Add(new Expense { UserID = _testUserId, Amount = 10m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "A" });
            _expenseRepo.Add(new Expense { UserID = _testUserId, Amount = 20m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "B" });

            var list = _expenseRepo.GetAll();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Update_Should_Modify_Existing_Expense()
        {
            var expense = new Expense { UserID = _testUserId, Amount = 50m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "Old" };
            _expenseRepo.Add(expense);
            var inserted = _expenseRepo.GetAll().Last();

            inserted.Amount = 75m;
            inserted.Description = "Updated";
            _expenseRepo.Update(inserted);

            var updated = _expenseRepo.GetById(inserted.ExpenseID);
            Assert.AreEqual(75m, updated.Amount);
            Assert.AreEqual("Updated", updated.Description);
        }

        [TestMethod]
        public void Delete_Should_Remove_Expense_Record()
        {
            var expense = new Expense { UserID = _testUserId, Amount = 30m, Date = DateTime.Today, CategoryID = _testCategoryId, Description = "ToDelete" };
            _expenseRepo.Add(expense);
            var inserted = _expenseRepo.GetAll().Last();

            _expenseRepo.Delete(inserted.ExpenseID);

            var fetched = _expenseRepo.GetById(inserted.ExpenseID);
            Assert.IsNull(fetched);
        }
    }
}
