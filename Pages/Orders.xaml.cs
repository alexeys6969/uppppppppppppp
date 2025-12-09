using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using up.Models;

namespace up.Pages
{
    /// <summary>
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Models.SupplierOrder> _orders;
        public Orders(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                var loadedOrders = Connection.GetOrders(connect);

                _orders = new ObservableCollection<Models.SupplierOrder>(loadedOrders);
                ordersDataGrid.ItemsSource = _orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(connect));
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            try
            {
                ordersDataGrid.CommitEdit();

                var currentOrders = _orders.ToList();
                var originOrders = Connection.GetOrders(connect).ToList();

                foreach (var orders in currentOrders)
                {
                    if (orders.Id > 0) // Существующая запись
                    {
                        var originalOrder = originOrders.FirstOrDefault(c => c.Id == orders.Id);
                        if (originalOrder != null &&
                            (originalOrder.SupplierName != orders.SupplierName ||
                    originalOrder.OrderDate != orders.OrderDate || originalOrder.StatusOrder != orders.StatusOrder || originalOrder.TotalCost != orders.TotalCost))
                        {
                            bool updated = Connection.UpdateOrder(orders, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить поставку '{orders.SupplierName}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var orders in currentOrders.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddOrder(orders, connect);
                    if (newId > 0)
                    {
                        orders.Id = newId;
                    }
                }

                var ordersToDelete = originOrders.Where(oldOrd =>
                    !currentOrders.Any(newOrd => newOrd.Id == oldOrd.Id)).ToList();

                foreach (var ordToDelete in ordersToDelete)
                {
                    bool deleted = Connection.DeleteOrder(ordToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить поставку '{ordToDelete.SupplierName}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadOrders();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadOrders();
            }
        }
    }
}
