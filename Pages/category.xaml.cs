using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для Category.xaml
    /// </summary>
    public partial class Category : Page
    {
        private Connection Connection;
        private string userRole;
        List<Models.Category> categories;
        Employees employee;
        public Category(string userPosition, Employees _employees)
        {
            InitializeComponent();
            Connection = new Connection();
            userRole = userPosition;
            LoadCategories();
            employee = _employees;
        }

        private void LoadCategories()
        {
            try
            {
                string roleConnection = Connection.GetConnection(userRole);
                categories = Connection.GetCategories(roleConnection);
                categoriesDataGrid.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {

        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            categories = (List<Models.Category>)categoriesDataGrid.ItemsSource;
            Connection.AddCategory(new Models.Category(), userRole);
        }
    }
}
