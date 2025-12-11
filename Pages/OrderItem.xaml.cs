using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
            try
            {
                ordersDataGrid.CommitEdit();
                var currentOrders = _order.ToList();
                var originalOrders = Connection.GetOrder(connect).ToList();

                foreach (var orders in currentOrders)
                {
                    if (orders.Id > 0) // Существующая запись
                    {
                        var originalOrder = originalOrders.FirstOrDefault(c => c.Id == orders.Id);
                        if (originalOrder != null &&
                            (originalOrder.order_id != orders.order_id ||
                             originalOrder.product_name != orders.product_name ||
                             originalOrder.quantity != orders.quantity ||
                             originalOrder.price != orders.price))
                        {
                            bool updated = Connection.UpdateOrder(orders, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить товар '{orders.product_name}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var orders in currentOrders.Where(c => c.Id == 0))
                {
                    try
                    {
                        int newId = Connection.AddOrderItem(orders, connect);
                        if (newId > 0)
                        {
                            orders.Id = newId;
                        }
                    } catch(SqlException ex)
                    {
                        MessageBox.Show($"Не удалось обработать запрос",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }

                var ordersToDelete = originalOrders.Where(oldOrd =>
                    !currentOrders.Any(newOrd => newOrd.Id == oldOrd.Id)).ToList();

                foreach (var ordToDelete in ordersToDelete)
                {
                    bool deleted = Connection.DeleteOrderItem(ordToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить товар '{ordToDelete.product_name}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadOrderItem();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadOrderItem();
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(connect));
        }
    }
}
