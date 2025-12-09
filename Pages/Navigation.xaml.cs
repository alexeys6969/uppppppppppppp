using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Navigation.xaml
    /// </summary>
    public partial class Navigation : Page
    {
        public string connection;
        public Navigation(string _connection)
        {
            InitializeComponent();
            connection = _connection;
        }

        private void CategoryClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Category(connection));
        }

        private void EmployeeClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.employees(connection));
        }

        private void ProductClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Products(connection));
        }

        private void ReportClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Reports(connection));
        }

        private void ReturnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Return(connection));
        }

        private void SaleClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Sales(connection));
        }

        private void SupplierClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.supplier(connection));
        }

        private void OrderClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Orders(connection));
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Authorization());
        }
    }
}
