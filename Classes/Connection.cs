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
using up.Pages;

namespace up.Classes
{
    public class Connection
    {

        #region Category
        public List<Models.Category> GetCategories(string roleConnectionString)
        {
            List<Models.Category> categories = new List<Models.Category>();
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
                        Models.Category category = new Models.Category
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

        public int AddCategory(Models.Category category, string Connection)
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

        #region Employees

        public List<Employees> GetEmployees(string roleConnectionString)
        {
            List<Employees> employees = new List<Employees>();
            string connectionString = roleConnectionString;
            string query;
            if (!connectionString.Contains("admin"))
            {
                query = "SELECT employee_id, full_name as 'ФИО сотрудника', position as 'Должность' FROM employee";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employees em = new Employees
                            {
                                employee_id = reader.GetInt32(0),
                                full_name = reader.GetString(1),
                                position = reader.GetString(2),
                                login = "-",
                                password = "-"
                            };
                            employees.Add(em);
                        }
                    }
                }
            }
            else
            {
                query = "SELECT employee_id, full_name as 'ФИО сотрудника', position as 'Должность', login_employee as 'Логин', password_hash as 'Пароль'  FROM employee";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Employees em = new Employees
                            {
                                employee_id = reader.GetInt32(0),
                                full_name = reader.GetString(1),
                                position = reader.GetString(2),
                                login = reader.GetString(3),
                                password = reader.GetString(4)
                            };
                            employees.Add(em);
                        }
                    }
                }
            }
            return employees;
        }

        public int AddEmployees(Employees employees, string Connection)
        {
            string query = @"
            INSERT INTO employee (full_name, position, login_employee, password_hash) 
            VALUES (@Name, @Position, @Login, @Password);
        ";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", employees.full_name);
                    command.Parameters.AddWithValue("@Position", employees.position);
                    command.Parameters.AddWithValue("@Login", employees.login);
                    command.Parameters.AddWithValue("@Password", HashPassword(employees.password));

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateEmployee(Employees employees, string userRole)
        {
            string connectionString = userRole;

            string query = @"UPDATE employee SET full_name = @Name, position = @Position, login_employee = @Login, password_hash = @Password WHERE employee_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", employees.employee_id);
                    command.Parameters.AddWithValue("@Name", employees.full_name);
                    command.Parameters.AddWithValue("@Position", employees.position);
                    command.Parameters.AddWithValue("@Login", employees.login);
                    command.Parameters.AddWithValue("@Password", HashPassword(employees.password));

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteEmployees(int employeesId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM employee WHERE employee_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", employeesId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        #endregion

        
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
