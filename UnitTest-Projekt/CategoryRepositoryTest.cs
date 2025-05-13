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
    public class CategoryRepositoryTests
    {
        private CategoryRepository _categoryRepo;

        [TestInitialize]
        public void Setup()
        {
            // Datenbank initialisieren und Tabellen anlegen
            DatabaseManager.InitializeDatabase();

            _categoryRepo = new CategoryRepository();

            // Income- und Expense-Tabellen bereinigen, damit FK von Category keine Löschung verhindert
            using (var conn = DatabaseManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM Income;", conn))
                    cmd.ExecuteNonQuery();
                using (var cmd = new MySqlCommand("DELETE FROM Expense;", conn))
                    cmd.ExecuteNonQuery();
                using (var cmd = new MySqlCommand("DELETE FROM Category;", conn))
                    cmd.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void Add_Should_Insert_Category_Record()
        {
            var category = new Category
            {
                Name = "TestKat",
                Type = CategoryType.Income
            };
            _categoryRepo.Add(category);

            var all = _categoryRepo.GetAll();
            Assert.IsTrue(all.Any(c => c.Name == "TestKat" && c.Type == CategoryType.Income));
        }

        [TestMethod]
        public void GetById_Should_Return_Correct_Category()
        {
            // erst anlegen
            var cat = new Category { Name = "GetByIdKat", Type = CategoryType.Expense };
            _categoryRepo.Add(cat);
            var inserted = _categoryRepo.GetAll().Last();

            // abrufen
            var fetched = _categoryRepo.GetById(inserted.CategoryID);
            Assert.IsNotNull(fetched);
            Assert.AreEqual("GetByIdKat", fetched.Name);
            Assert.AreEqual(CategoryType.Expense, fetched.Type);
        }

        [TestMethod]
        public void GetAll_Should_Return_List_Of_Categories()
        {
            _categoryRepo.Add(new Category { Name = "A", Type = CategoryType.Income });
            _categoryRepo.Add(new Category { Name = "B", Type = CategoryType.Expense });

            var list = _categoryRepo.GetAll();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Update_Should_Modify_Existing_Category()
        {
            var cat = new Category { Name = "OldName", Type = CategoryType.Income };
            _categoryRepo.Add(cat);
            var inserted = _categoryRepo.GetAll().Last();

            // ändern
            inserted.Name = "NewName";
            inserted.Type = CategoryType.Expense;
            _categoryRepo.Update(inserted);

            var updated = _categoryRepo.GetById(inserted.CategoryID);
            Assert.AreEqual("NewName", updated.Name);
            Assert.AreEqual(CategoryType.Expense, updated.Type);
        }

        [TestMethod]
        public void Delete_Should_Remove_Category_Record()
        {
            var cat = new Category { Name = "ToDelete", Type = CategoryType.Income };
            _categoryRepo.Add(cat);
            var inserted = _categoryRepo.GetAll().Last();
            _categoryRepo.Delete(inserted.CategoryID);

            var fetched = _categoryRepo.GetById(inserted.CategoryID);
            Assert.IsNull(fetched);
        }
    }
}
