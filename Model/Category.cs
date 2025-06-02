using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Freelance_Finance_Manager
{
    // 1. Enum für den Typ der Kategorie
    public enum CategoryType
    {
        Income,
        Expense
    }

    // 2. Model-Klasse für Category
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }
    }

    // 3. Repository-Klasse
    public class CategoryRepository : BaseRepository<Category>
    {
        public override void Add(Category category)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "INSERT INTO Category (Name, Type) VALUES (@Name, @Type); SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Type", category.Type.ToString());

                    var insertedId = Convert.ToInt32(command.ExecuteScalar());
                    category.CategoryID = insertedId; // ID im Objekt setzen
                }
            }
        }


        public override Category GetById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Category WHERE CategoryID = @CategoryID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Category
                            {
                                CategoryID = reader.GetInt32("CategoryID"),
                                Name = reader.GetString("Name"),
                                Type = (CategoryType) Enum.Parse(typeof(CategoryType), reader.GetString("Type"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public override List<Category> GetAll()
        {
            var categories = new List<Category>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Category;";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryID = reader.GetInt32("CategoryID"),
                            Name = reader.GetString("Name"),
                            Type = (CategoryType)Enum.Parse(typeof(CategoryType), reader.GetString("Type"))
                        });
                    }
                }
            }
            return categories;
        }

        public override void Update(Category category)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE Category SET Name = @Name, Type = @Type WHERE CategoryID = @CategoryID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Type", category.Type.ToString());
                    command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM Category WHERE CategoryID = @CategoryID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Category GetByNameAndType(string name, CategoryType type)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM Category WHERE Name = @Name AND Type = @Type;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Type", type.ToString());

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Category
                            {
                                CategoryID = reader.GetInt32("CategoryID"),
                                Name = reader.GetString("Name"),
                                Type = (CategoryType)Enum.Parse(typeof(CategoryType), reader.GetString("Type"))
                            };
                        }
                        return null;
                    }
                }
            }
        }

    }
}
