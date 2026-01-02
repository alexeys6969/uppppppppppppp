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
    /// Логика взаимодействия для SaleItem.xaml
    /// </summary>
    public partial class SaleItem : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Models.SaleItem> _sitems;
        public SaleItem(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadItms();
        }

        private void LoadItms()
        {
            try
            {
                var loadedItems = Connection.GetSaleItm(connect);

                _sitems = new ObservableCollection<Models.SaleItem>(loadedItems);
                salesItemsDataGrid.ItemsSource = _sitems;
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
                salesItemsDataGrid.CommitEdit();
                var currentSItems = _sitems.ToList();
                var originalSItems = Connection.GetSaleItm(connect).ToList();

                foreach (var SItems in currentSItems)
                {
                    if (SItems.Id > 0) // Существующая запись
                    {
                        var originalSItem = originalSItems.FirstOrDefault(c => c.Id == SItems.Id);
                        if (originalSItem != null &&
                            (originalSItem.product_name != SItems.product_name ||
                             originalSItem.sale_number != SItems.sale_number ||
                             originalSItem.quantity != SItems.quantity ||
                             originalSItem.price != SItems.price))
                        {
                            bool updated = Connection.UpdateSaleItm(SItems, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить позицию продажи '{SItems.sale_number}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var sitems in currentSItems.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddSaleItm(sitems, connect);
                    if (newId > 0)
                    {
                        sitems.Id = newId;
                    }
                }

                var sitemsToDelete = originalSItems.Where(oldSIT =>
                    !currentSItems.Any(newSIT => newSIT.Id == oldSIT.Id)).ToList();

                foreach (var siteToDelete in sitemsToDelete)
                {
                    bool deleted = Connection.DeleteSaleItm(siteToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить позицию продажи '{siteToDelete.sale_number}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadItms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadItms();
            }
        }
    }
}
