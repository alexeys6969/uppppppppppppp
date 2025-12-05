using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using up.Classes;

namespace up.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        Employees employee;
        Connection connect = new Connection();
        public Authorization(Employees _employee)
        {
            InitializeComponent();
            employee = _employee;
        }

        private void authorization(object sender, RoutedEventArgs e)
        {
            string Login = login.Text.Trim();
            string Password = password.Password;

            Connection db = new Connection();

            DataTable employeesDT = db.ExecuteQuery("SELECT * FROM employee");

            bool found = false;
            Employees currentEmployee = null;

            foreach (DataRow row in employeesDT.Rows)
            {
                string dbLogin = row["login_employee"].ToString();
                string dbPasswordHash = row["password_hash"].ToString();
                string hashedInput = db.HashPassword(Password);

                if (Login == dbLogin && hashedInput == dbPasswordHash)
                {
                    found = true;
                    currentEmployee = new Employees
                    {
                        employee_id = Convert.ToInt32(row["employee_id"]),
                        full_name = row["full_name"].ToString(),
                        position = row["position"].ToString(),
                        login = dbLogin,
                        password = dbPasswordHash
                    };
                    break;
                }
            }

            if (found && currentEmployee != null)
            {
                string roleConnection = db.GetConnection(currentEmployee.position);
                MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(currentEmployee));

                try
                {
                    using (var conn = new SqlConnection(roleConnection))
                    {
                        conn.Open();

                        currentEmployee.password = roleConnection;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка подключения: {ex.Message}\n\n" +
                                  "Возможно, не созданы пользователи SQL Server для ролей.",
                                  "Ошибка прав",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль",
                              "Ошибка входа",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                login.Text = "";
                password.Password = "";
            }
        }

        private void registration(object sender, RoutedEventArgs e)
        {

        }
    } 
}
            
