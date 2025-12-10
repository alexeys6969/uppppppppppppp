using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
using up.Models;

namespace up.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderItem.xaml
    /// </summary>
    public partial class OrderItem : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<SupplierOrderItem> _order;
        public OrderItem(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadOrderItem();
        }

        private void LoadOrderItem()
        {
            try
            {
                var loadedItems = Connection.GetOrder(connect);

                _order = new ObservableCollection<SupplierOrderItem>(loadedItems);
                ordersDataGrid.ItemsSource = _order;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message.ToString()}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {

        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(connect));
        }
    }
}
