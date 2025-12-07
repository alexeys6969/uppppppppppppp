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
        public Authorization()
        {
            InitializeComponent();
        }

        private void authorization(object sender, RoutedEventArgs e)
        {
            string dbUser = login.Text.Trim();
            string dbPass = password.Password.Trim();

            if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass))
            {
                MessageBox.Show("Введите логин и пароль для подключения к серверу", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tempConnString = CurrentUser.BuildConnectionString(dbUser, dbPass);

            try
            {
                using (var conn = new SqlConnection(tempConnString))
                {
                    conn.Open();
                }

                CurrentUser.ActiveConnectionString = tempConnString;

                // Переходим на окно авторизации
                MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(CurrentUser.ActiveConnectionString));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к SQL Server:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    } 
}
            
