using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using up.Models;

namespace up.Classes
{
    public class Connection
    {

        private string masterConnection = "Server=WINSERV-ISP-23-\\WINSERVSQL; Database=music_store; User Id=sa; Password=Asdfg123_; TrustServerCertificate=true;";

        public DataRow CheckLogin(string login, string password)
        {
            string hashedPassword = HashPassword(password);

            string query = @"
                SELECT employee_id, full_name, position, login_employee 
                FROM employee 
                WHERE login_employee = @login AND password_hash = @password
            ";

            using (SqlConnection connection = new SqlConnection(masterConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", hashedPassword);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                            return table.Rows[0];
                    }
                }
            }
            return null;
        }

        public string GetConnection(string position)
        {
            switch (position.ToLower())
            {
                case "администратор":
                    return "Server=WINSERV-ISP-23-\\WINSERVSQL; Database=music_store; User Id=admin_music_store; Password=Admin@12345; TrustServerCertificate=true;";

                case "менеджер":
                    return "Server=WINSERV-ISP-23-\\WINSERVSQL; Database=music_store; User Id=manager_music_store; Password=Manager@12345; TrustServerCertificate=true;";

                case "кассир":
                    return "Server=WINSERV-ISP-23-\\WINSERVSQL; Database=music_store; User Id=cashier_music_store; Password=Cashier@12345; TrustServerCertificate=true;";

                default:
                    return masterConnection;
            }
        }

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

        public DataTable ExecuteQuery(string query)
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(masterConnection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        // Метод для проверки, что подключение с правами работает
        public bool TestRoleConnection(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Простой запрос для проверки
                    using (SqlCommand command = new SqlCommand("SELECT 1", connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null && Convert.ToInt32(result) == 1;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Метод для выполнения запроса с правами конкретной роли
        public DataTable ExecuteWithRole(string query, string roleConnectionString)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(roleConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

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
