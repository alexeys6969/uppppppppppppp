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

namespace up
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Employees currentEmployee;
        private string connectionString;
        private Connection db;
        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(new Pages.Authorization(currentEmployee));
        }
        //public MainWindow(Employees employee, string connectionString)
        //{
        //    InitializeComponent();
        //    this.currentEmployee = employee;
        //    this.connectionString = connectionString;
        //    this.db = new Connection();

        //    this.Title = $"Music Store - {employee.full_name} ({employee.position})";
        //}
    }
}
