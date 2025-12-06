using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для Sales.xaml
    /// </summary>
    public partial class Sales : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Sale> _sale;
        public Sales(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
                var loadedSales = Connection.GetSale(connect);

                _sale = new ObservableCollection<Sale>(loadedSales);
                salesDataGrid.ItemsSource = _sale;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message.ToString()}",
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
                salesDataGrid.CommitEdit();
                var currentSales = _sale.ToList();
                var originalSales = Connection.GetSale(connect).ToList();

                foreach (var sales in currentSales)
                {
                    if (sales.Id > 0) // Существующая запись
                    {
                        var originalSale = originalSales.FirstOrDefault(c => c.Id == sales.Id);
                        if (originalSale != null &&
                            (originalSale.SaleNumber != sales.SaleNumber ||
                             originalSale.Id != sales.Id))
                        {
                            bool updated = Connection.UpdateSale(sales, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить продажу '{sales.SaleNumber}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var sales in currentSales.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddSale(sales, connect);
                    if (newId > 0)
                    {
                        sales.Id = newId;
                    }
                }

                var salesToDelete = originalSales.Where(oldSale =>
                    !currentSales.Any(newSale => newSale.Id == oldSale.Id)).ToList();

                foreach (var saleToDelete in salesToDelete)
                {
                    bool deleted = Connection.DeleteSale(saleToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить продажу '{saleToDelete.SaleNumber}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadSales();
            }
        }
    }
}
