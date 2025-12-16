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

        }
    }
}
