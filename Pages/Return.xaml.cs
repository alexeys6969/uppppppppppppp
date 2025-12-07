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
    /// Логика взаимодействия для Return.xaml
    /// </summary>
    public partial class Return : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Returns> _return;
        public Return(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadReturns();
        }

        private void LoadReturns()
        {
            try
            {
                var loadedReturns = Connection.GetReturn(connect);

                _return = new ObservableCollection<Returns>(loadedReturns);
                returnsDataGrid.ItemsSource = _return;
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
                returnsDataGrid.CommitEdit();
                var currentReturns = _return.ToList();
                var originalReturns = Connection.GetReturn(connect).ToList();

                foreach (var returns in currentReturns)
                {
                    if (returns.Id > 0) // Существующая запись
                    {
                        var originReturn = originalReturns.FirstOrDefault(c => c.Id == returns.Id);
                        if (originReturn != null &&
                            (originReturn.ReturnNumber != returns.ReturnNumber))
                        {
                            bool updated = Connection.UpdateReturn(returns, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить возврат '{returns.SaleNumber}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var returns in currentReturns.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddReturn(returns, connect);
                    if (newId > 0)
                    {
                        returns.Id = newId;
                    }
                }

                var returnsToDelete = originalReturns.Where(oldRet =>
                    !currentReturns.Any(newRet => newRet.Id == oldRet.Id)).ToList();

                foreach (var retToDelete in returnsToDelete)
                {
                    bool deleted = Connection.DeleteReturn(retToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить возврат '{retToDelete.SaleNumber}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadReturns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadReturns();
            }
        }
    }
}
