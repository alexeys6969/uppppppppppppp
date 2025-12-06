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

        #region Product

        public List<Product> GetProduct(string roleConnectionString)
        {
            List<Product> products = new List<Product>();
            string connectionString = roleConnectionString;
            string query = @"
        SELECT product_id, article, name_product, brand, model, price, 
               quantity_in_stock, category.category_id, name_category 
        FROM product 
        INNER JOIN category ON product.category_id = category.category_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Article = reader.GetString(1),
                            NameProduct = reader.GetString(2),
                            Brand = reader.GetString(3),
                            Model = reader.GetString(4),
                            Price = reader.GetDecimal(5),
                            QuantityStock = reader.GetInt32(6),
                            CategoryId = reader.GetInt32(7),
                            CategoryName = reader.GetString(8)
                        };
                        products.Add(product);
                    }
                }
            }
            return products;
        }

        public int AddProduct(Product product, string Connection)
        {
            string query = @"
            INSERT INTO product (article, name_product, brand, model, price, quantity_in_stock, category_id)
            VALUES (
            @article, 
            @name_product, 
            @brand, 
            @model, 
            @price, 
            @quantity_in_stock,
            (SELECT category_id FROM category WHERE name_category = @category_name))";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@article", product.Article);
                    command.Parameters.AddWithValue("@name_product", product.NameProduct);
                    command.Parameters.AddWithValue("@brand", product.Brand);
                    command.Parameters.AddWithValue("@model", product.Model);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@quantity_in_stock", product.QuantityStock);
                    command.Parameters.AddWithValue("@category_name", product.CategoryName);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateProduct(Product product, string userrole)
        {
            string connectionstring = userrole;

            string query = @"UPDATE product 
            SET 
            article = @article,
            name_product = @name_product,
            brand = @brand,
            model = @model,
            price = @price,
            quantity_in_stock = @quantity_in_stock,
            category_id = (SELECT category_id FROM category WHERE name_category = @category_name)
            WHERE product_id = @product_id;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@product_id", product.Id);
                    command.Parameters.AddWithValue("@article", product.Article);
                    command.Parameters.AddWithValue("@name_product", product.NameProduct);
                    command.Parameters.AddWithValue("@brand", product.Brand);
                    command.Parameters.AddWithValue("@model", product.Model);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@quantity_in_stock", product.QuantityStock);
                    command.Parameters.AddWithValue("@category_name", product.CategoryName);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        //// Удаление категории
        public bool DeleteProduct(int productId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM product WHERE product_id = @product_id;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@product_id", productId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
        #endregion

        #region Report

        public List<Report> GetReport(string roleConnectionString)
        {
            List<Report> products = new List<Report>();
            string connectionString = roleConnectionString;
            string query = @"SELECT report_id, report_type, period_start, period_end, created_at, full_name 
        FROM report
        INNER JOIN employee ON report.created_by = employee.employee_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Report report = new Report
                        {
                            Id = reader.GetInt32(0),
                            ReportType = reader.GetString(1),
                            PeriodStart = reader.GetDateTime(2),
                            PeriodEnd = reader.GetDateTime(3),
                            CreatedAt = reader.GetDateTime(4),
                            NameEmployee = reader.GetString(5)
                        };
                        products.Add(report);
                    }
                }
            }
            return products;
        }

        public int AddReport(Report report, string Connection)
        {
            string query = @"
            INSERT INTO report (report_type, period_start, period_end, created_at, created_by)
            VALUES (
            @Type, 
            @Start, 
            @End, 
            @At,
            (SELECT employee_id FROM employee WHERE full_name = @Name))";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", report.ReportType);
                    command.Parameters.AddWithValue("@Start", report.PeriodStart);
                    command.Parameters.AddWithValue("@End", report.PeriodEnd);
                    command.Parameters.AddWithValue("@At", report.CreatedAt);
                    command.Parameters.AddWithValue("@Name", report.NameEmployee);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateReport(Report report, string userrole)
        {
            string connectionstring = userrole;

            string query = @"UPDATE report 
                 SET 
                    report_type = @Type,
                    period_start = @Start,
                    period_end = @End,
                    created_at = @At,
                    created_by = (SELECT employee_id FROM employee WHERE full_name = @Name)
                 WHERE report_id = @Id;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", report.Id);
                    command.Parameters.AddWithValue("@Type", report.ReportType);
                    command.Parameters.AddWithValue("@Start", report.PeriodStart);
                    command.Parameters.AddWithValue("@End", report.PeriodEnd);
                    command.Parameters.AddWithValue("@At", report.CreatedAt);
                    command.Parameters.AddWithValue("@Name", report.NameEmployee);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteReport(int reportId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM report WHERE report_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", reportId);
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
