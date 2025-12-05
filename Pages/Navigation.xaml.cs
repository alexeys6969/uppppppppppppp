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
        Employees currentEmployee;
        Connection NavConnect = new Connection();
        private string role;
        public Navigation(Employees _employee)
        {
            InitializeComponent();
            currentEmployee = _employee;
            role = NavConnect.GetConnection(currentEmployee.position);
            navName.Content += currentEmployee.full_name;
        }

        private void CategoryClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Category(role, currentEmployee));
        }

        private void EmployeeClick(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.employees(role, currentEmployee));
        }

        private void ProductClick(object sender, RoutedEventArgs e)
        {

        }

        private void ReportClick(object sender, RoutedEventArgs e)
        {

        }

        private void ReturnClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaleClick(object sender, RoutedEventArgs e)
        {

        }

        private void SupplierClick(object sender, RoutedEventArgs e)
        {

        }

        private void OrderClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
