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
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Product> _product;
        public Products(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var loadedProducts = Connection.GetProduct(connect);

                _product = new ObservableCollection<Product>(loadedProducts);
                productsDataGrid.ItemsSource = _product;
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
                productsDataGrid.CommitEdit();
                var currentProducts = _product.ToList();
                var originalProducts = Connection.GetProduct(connect).ToList();

                foreach (var products in currentProducts)
                {
                    if (products.Id > 0) // Существующая запись
                    {
                        var originalProduct = originalProducts.FirstOrDefault(c => c.Id == products.Id);
                        if (originalProduct != null &&
                            (originalProduct.NameProduct != products.NameProduct ||
                             originalProduct.Article != products.Article))
                        {
                            bool updated = Connection.UpdateProduct(products, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить товар '{products.NameProduct}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var products in currentProducts.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddProduct(products, connect);
                    if (newId > 0)
                    {
                        products.Id = newId;
                    }
                }

                var productsToDelete = originalProducts.Where(oldProd =>
                    !currentProducts.Any(newProd => newProd.Id == oldProd.Id)).ToList();

                foreach (var prodlToDelete in productsToDelete)
                {
                    bool deleted = Connection.DeleteProduct(prodlToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить товар '{prodlToDelete.NameProduct}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadProducts();
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(connect));
        }
    }
}
