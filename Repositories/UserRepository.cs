using Freelance_Finance_Manager.Repositories;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Freelance_Finance_Manager
{

    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    public class UserRepository : BaseRepository<User>
    {
        public override void Add(User user)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "INSERT INTO User (Name, Email, PasswordHash) VALUES (@Name, @Email, @PasswordHash);";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override User GetById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM User WHERE UserID = @UserID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32("UserID"),
                                Name = reader.GetString("Name"),
                                Email = reader.GetString("Email"),
                                PasswordHash = reader.GetString("PasswordHash")
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public override List<User> GetAll()
        {
            var users = new List<User>();
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "SELECT * FROM User;";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserID = reader.GetInt32("UserID"),
                            Name = reader.GetString("Name"),
                            Email = reader.GetString("Email"),
                            PasswordHash = reader.GetString("PasswordHash")
                        });
                    }
                }
            }
            return users;
        }

        public override void Update(User user)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "UPDATE User SET Name = @Name, Email = @Email, PasswordHash = @PasswordHash WHERE UserID = @UserID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.ExecuteNonQuery();
                }
            }
        }

        public override void Delete(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var query = "DELETE FROM User WHERE UserID = @UserID;";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
