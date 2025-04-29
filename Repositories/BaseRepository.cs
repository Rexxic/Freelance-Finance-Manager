using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Freelance_Finance_Manager.Repositories
{
    public abstract class BaseRepository<T>
    {
        protected MySqlConnection GetConnection()
        {
            return DatabaseManager.GetConnection();
        }

        public abstract void Add(T entity);
        public abstract T GetById(int id);
        public abstract List<T> GetAll();
        public abstract void Update(T entity);
        public abstract void Delete(int id);
    }
}