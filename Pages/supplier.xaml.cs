using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для supplier.xaml
    /// </summary>
    public partial class supplier : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Models.Supplier> _supplier;
        public supplier(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                var loadedSuppliers = Connection.GetSupplier(connect);

                _supplier = new ObservableCollection<Supplier>(loadedSuppliers);
                suppliersDataGrid.ItemsSource = _supplier;
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
                suppliersDataGrid.CommitEdit();
                suppliersDataGrid.CommitEdit();

                var currentSuppliers = _supplier.ToList();
                var originaSuppliers = Connection.GetSupplier(connect).ToList();

                foreach (var suppliers in currentSuppliers)
                {
                    if (suppliers.Id > 0) // Существующая запись
                    {
                        var originalSupplier = originaSuppliers.FirstOrDefault(c => c.Id == suppliers.Id);
                        if (originalSupplier != null &&
                            (originalSupplier.NameSupplier != suppliers.NameSupplier ||
                    originalSupplier.ContactInfo != suppliers.ContactInfo))
                        {
                            bool updated = Connection.UpdateSupplier(suppliers, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить поставщика '{suppliers.NameSupplier}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var suppliers in currentSuppliers.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddSupplier(suppliers, connect);
                    if (newId > 0)
                    {
                        suppliers.Id = newId;
                    }
                }

                var suppliersToDelete = originaSuppliers.Where(oldSup =>
                    !currentSuppliers.Any(newSup => newSup.Id == oldSup.Id)).ToList();

                foreach (var supToDelete in suppliersToDelete)
                {
                    bool deleted = Connection.DeleteSupplier(supToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить поставщика '{supToDelete.NameSupplier}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadSuppliers();
            }
        }
    }
}
