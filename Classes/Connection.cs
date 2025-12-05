using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using up.Models;

namespace up.Classes
{
    public class Connection
    {

        #region Category
        public List<Category> GetCategories(string roleConnectionString)
        {
            List<Category> categories = new List<Category>();
            string connectionString = roleConnectionString;
            string query = "SELECT * FROM category";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Category category = new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2)
                        };
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }

        public int AddCategory(Category category, string Connection)
        {
            string query = @"
            INSERT INTO category (name_category, description_category) 
            VALUES (@Name, @Description);
        ";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Description", category.Description);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateCategory(Models.Category category, string userRole)
        {
            string connectionString = userRole;

            string query = @"
        UPDATE category 
        SET name_category = @Name, 
            description_category = @Description 
        WHERE category_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", category.Id);
                    command.Parameters.AddWithValue("@Name", category.Name);
                    command.Parameters.AddWithValue("@Description", category.Description);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteCategory(int categoryId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM category WHERE category_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", categoryId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        #endregion

        //public List<Employees> GetEmployees(string roleConnectionString)
        //{
        //    List<Employees> employees = new List<Employees>();
        //    string connectionString = roleConnectionString;
        //    string query = "SELECT full_name, position FROM employee";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                Employees em = new Employees
        //                {
        //                    full_name = reader.GetString(0),
        //                    position = reader.GetString(1)
        //                };
        //                employees.Add(em);
        //            }
        //        }
        //    }
        //    return employees;
        //}





        //public DataTable ExecuteQuery(string query)
        //{
        //    DataTable table = new DataTable();

        //    using (SqlConnection connection = new SqlConnection(masterConnection))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //        {
        //            adapter.Fill(table);
        //        }
        //    }

        //    return table;
        //}

        //// Метод для проверки, что подключение с правами работает
        //public bool TestRoleConnection(string connectionString)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            // Простой запрос для проверки
        //            using (SqlCommand command = new SqlCommand("SELECT 1", connection))
        //            {
        //                object result = command.ExecuteScalar();
        //                return result != null && Convert.ToInt32(result) == 1;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //// Метод для выполнения запроса с правами конкретной роли
        //public DataTable ExecuteWithRole(string query, string roleConnectionString)
        //{
        //    DataTable dataTable = new DataTable();

        //    using (SqlConnection connection = new SqlConnection(roleConnectionString))
        //    {
        //        connection.Open();
        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //        {
        //            adapter.Fill(dataTable);
        //        }
        //    }

        //    return dataTable;
        //}

        // Хеширование пароля
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString().ToUpper();
            }
        }
    }
}
