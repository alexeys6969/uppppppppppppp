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

        #region Sale

        public List<Sale> GetSale(string roleConnectionString)
        {
            List<Sale> sales = new List<Sale>();
            string connectionString = roleConnectionString;
            string query = @"SELECT sale_id, sale_number, full_name, sale_date, total_amount, payment_status, payment_method 
        FROM sale
        INNER JOIN employee ON sale.employee_id = employee.employee_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Sale sale = new Sale
                        {
                            Id = reader.GetInt32(0),
                            SaleNumber = reader.GetString(1),
                            EmployeeName = reader.GetString(2),
                            SaleDate = reader.GetDateTime(3),
                            TotalAmount = reader.GetDecimal(4),
                            PaymentStatus = reader.GetString(5),
                            PaymentMethod = reader.GetString(6)
                        };
                        sales.Add(sale);
                    }
                }
            }
            return sales;
        }

        public int AddSale(Sale sale, string Connection)
        {
            string query = @"
            INSERT INTO sale (sale_number, employee_id, sale_date, total_amount, payment_status, payment_method)
            VALUES (
            @Number,
            (SELECT employee_id FROM employee WHERE full_name = @Name),
            @Date, 
            @Amount,
            @Status,
            @Method
            )";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Number", sale.SaleNumber);
                    command.Parameters.AddWithValue("@Name", sale.EmployeeName);
                    command.Parameters.AddWithValue("@Date", sale.SaleDate);
                    command.Parameters.AddWithValue("@Amount", sale.TotalAmount);
                    command.Parameters.AddWithValue("@Status", sale.PaymentStatus);
                    command.Parameters.AddWithValue("@Method", sale.PaymentMethod);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateSale(Sale sale, string userrole)
        {
            string connectionstring = userrole;

            string query = @"UPDATE sale 
                 SET 
                    sale_number = @Number,
                    employee_id = (SELECT employee_id FROM employee WHERE full_name = @Name),                    
                    sale_date = @Date,
                    total_amount = @Amount,
                    payment_status = @Status,
                    payment_method = @Method
                 WHERE sale_id = @Id;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", sale.Id);
                    command.Parameters.AddWithValue("@Number", sale.SaleNumber);
                    command.Parameters.AddWithValue("@Date", sale.SaleDate);
                    command.Parameters.AddWithValue("@Amount", sale.TotalAmount);
                    command.Parameters.AddWithValue("@Status", sale.PaymentStatus);
                    command.Parameters.AddWithValue("@Method", sale.PaymentMethod);
                    command.Parameters.AddWithValue("@Name", sale.EmployeeName);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteSale(int saleId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM sale WHERE sale_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", saleId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Return

        public List<Returns> GetReturn(string roleConnectionString)
        {
            List<Returns> returns = new List<Returns>();
            string connectionString = roleConnectionString;
            string query = @"SELECT return_id, sale.sale_number, employee.full_name, return_number, return_date, reason, total_refund
        FROM returns
        INNER JOIN sale ON returns.sale_id = sale.sale_id
		INNER JOIN employee ON returns.employee_id = employee.employee_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Returns _return = new Returns
                        {
                            Id = reader.GetInt32(0),
                            SaleNumber = reader.GetString(1),
                            EmployeeName = reader.GetString(2),
                            ReturnNumber = reader.GetString(3),
                            ReturnDate = reader.GetDateTime(4),
                            Reason = reader.GetString(5),
                            TotalRefund = reader.GetDecimal(6)
                        };
                        returns.Add(_return);
                    }
                }
            }
            return returns;
        }

        public int AddReturn(Returns returns, string Connection)
        {
            string query = @"
            INSERT INTO returns (sale_id, employee_id, return_number, return_date, reason, total_refund)
            SELECT 
            (SELECT sale_id FROM sale WHERE sale_number = @sale_number),
            (SELECT employee_id FROM employee WHERE full_name = @employee_name),
            @return_number,
            @return_date,
            @reason,
            @total_refund;";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sale_number", returns.SaleNumber);
                    command.Parameters.AddWithValue("@employee_name", returns.EmployeeName);
                    command.Parameters.AddWithValue("@return_number", returns.ReturnNumber);
                    command.Parameters.AddWithValue("@return_date", returns.ReturnDate);
                    command.Parameters.AddWithValue("@reason", returns.Reason);
                    command.Parameters.AddWithValue("@total_refund", returns.TotalRefund);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateReturn(Returns returns, string userrole)
        {
            string connectionstring = userrole;

            string query = @"UPDATE returns 
            SET 
            sale_id = (SELECT sale_id FROM sale WHERE sale_number = @sale_number),
            employee_id = (SELECT employee_id FROM employee WHERE full_name = @employee_name),
            return_number = @return_number,
            return_date = @return_date,
            reason = @reason,
            total_refund = @total_refund
            WHERE return_id = @Id;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", returns.Id);
                    command.Parameters.AddWithValue("@sale_number", returns.SaleNumber);
                    command.Parameters.AddWithValue("@employee_name", returns.EmployeeName);
                    command.Parameters.AddWithValue("@return_number", returns.ReturnNumber);
                    command.Parameters.AddWithValue("@return_date", returns.ReturnDate);
                    command.Parameters.AddWithValue("@reason",  returns.Reason);
                    command.Parameters.AddWithValue("@total_refund", returns.TotalRefund);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteReturn(int returnId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM returns WHERE return_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", returnId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Supplier

        public List<Supplier> GetSupplier(string roleConnectionString)
        {
            List<Supplier> suppliers = new List<Supplier>();
            string connectionString = roleConnectionString;
            string query = @"SELECT * FROM supplier";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Supplier supplier = new Supplier
                        {
                            Id = reader.GetInt32(0),
                            NameSupplier = reader.GetString(1),
                            ContactInfo = reader.GetString(2)
                        };
                        suppliers.Add(supplier);
                    }
                }
            }
            return suppliers;
        }

        public int AddSupplier(Supplier supplier, string Connection)
        {
            string query = @"
            INSERT INTO supplier (name_supplier, contact_info) values (@Name, @Contact)";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", supplier.NameSupplier);
                    command.Parameters.AddWithValue("@Contact", supplier.ContactInfo);
 
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateSupplier(Supplier supplier, string userrole)
        {
            string connectionstring = userrole;

            string query = @"UPDATE supplier 
            SET 
            name_supplier = @Name,
            contact_info = @Contact
            where supplier_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", supplier.Id);
                    command.Parameters.AddWithValue("@Name", supplier.NameSupplier);
                    command.Parameters.AddWithValue("@Contact", supplier.ContactInfo);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteSupplier(int supplierId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM supplier WHERE supplier_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", supplierId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Supplier_order

        public List<SupplierOrder> GetOrders(string roleConnectionString)
        {
            List<SupplierOrder> orders = new List<SupplierOrder>();
            string connectionString = roleConnectionString;
            string query = @"SELECT order_id, supplier.name_supplier, order_date, status_order, total_cost
        FROM supplier_order
        INNER JOIN supplier ON supplier_order.supplier_id = supplier.supplier_id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupplierOrder order = new SupplierOrder
                        {
                            Id = reader.GetInt32(0),
                            SupplierName = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2),
                            StatusOrder = reader.GetString(3),
                            TotalCost = reader.GetDecimal(4)
                        };
                        orders.Add(order);
                    }
                }
            }
            return orders;
        }

        public int AddOrder(SupplierOrder supplier, string Connection)
        {
            string query = @"
            INSERT INTO supplier_order (supplier_id, order_date, status_order, total_cost) values (
            (SELECT supplier_id FROM supplier WHERE name_supplier = @Name),
            @Date, 
            @Status, 
            @Cost)";

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", supplier.SupplierName);
                    command.Parameters.AddWithValue("@Date", supplier.OrderDate);
                    command.Parameters.AddWithValue("@Status", supplier.StatusOrder);
                    command.Parameters.AddWithValue("@Cost", supplier.TotalCost);

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public bool UpdateOrder(SupplierOrder orders, string userrole)
        {
            string connectionstring = userrole;

            string query = @"UPDATE supplier_order 
            SET 
            supplier_id = (SELECT supplier_id FROM supplier WHERE name_supplier = @Name),
            order_date = @Date,
            status_order = @Status,
            total_cost = @Cost
            WHERE order_id = @Id;";

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", orders.Id);
                    command.Parameters.AddWithValue("@Name", orders.SupplierName);
                    command.Parameters.AddWithValue("@Date", orders.OrderDate);
                    command.Parameters.AddWithValue("@Status", orders.StatusOrder);
                    command.Parameters.AddWithValue("@Cost", orders.TotalCost);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Удаление категории
        public bool DeleteOrder(int orderId, string userRole)
        {
            string connectionString = userRole;

            string query = "DELETE FROM supplier_order WHERE order_id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", orderId);
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
