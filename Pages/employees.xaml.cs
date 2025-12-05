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
using System.Windows.Threading;
using up.Classes;

namespace up.Pages
{
    /// <summary>
    /// Логика взаимодействия для employees.xaml
    /// </summary>
    public partial class employees : Page
    {
        private Connection Connection;
        private string userRole;
        private ObservableCollection<Employees> _employees; 
        Employees employee;
        public employees(string userPosition, Employees _employees)
        {
            InitializeComponent();
            Connection = new Connection();
            userRole = userPosition;
            LoadEmployees();
            employee = _employees;
        }
        private void LoadEmployees()
        {
            try
            {
                string roleConnection = Connection.GetConnection(userRole);
                var loadedEmployees = Connection.GetEmployees(roleConnection);

                // Используем ObservableCollection для автоматического обновления UI
                _employees = new ObservableCollection<Employees>(loadedEmployees);
                employeesDataGrid.ItemsSource = _employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(employee));
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    // Принудительно завершаем редактирование в DataGrid
            //    employeesDataGrid.CommitEdit();

            //    // Делаем паузу для применения изменений
            //    Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

            //    // Сохраняем копию текущих данных
            //    var currentEmployees = _employees.ToList();
            //    var originalEmployees = Connection.GetEmployees(Connection.GetConnection(userRole)).ToList();

            //    // 1. ОБНОВЛЕНИЕ существующих категорий
            //    foreach (var employees in currentEmployees)
            //    {
            //        if (employees.employee_id > 0) // Существующая запись
            //        {
            //            var originalEmpl = originalEmployees.FirstOrDefault(c => c.employee_id == employees.employee_id);
            //            if (originalEmpl != null &&
            //                (originalEmpl.full_name != employees.full_name ||
            //                 originalEmpl.position != employees.position))
            //            {
            //                bool updated = Connection.UpdateCategory(category, userRole);
            //                if (!updated)
            //                {
            //                    MessageBox.Show($"Не удалось обновить категорию '{category.Name}'",
            //                                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            //                }
            //            }
            //        }
            //    }

            //    // 2. ДОБАВЛЕНИЕ новых категорий
            //    foreach (var category in currentCategories.Where(c => c.Id == 0))
            //    {
            //        int newId = Connection.AddCategory(category, userRole);
            //        if (newId > 0)
            //        {
            //            category.Id = newId;
            //        }
            //    }

            //    // 3. УДАЛЕНИЕ категорий
            //    var categoriesToDelete = originalCategories.Where(oldCat =>
            //        !currentCategories.Any(newCat => newCat.Id == oldCat.Id)).ToList();

            //    foreach (var categoryToDelete in categoriesToDelete)
            //    {
            //        bool deleted = Connection.DeleteCategory(categoryToDelete.Id, userRole);
            //        if (!deleted)
            //        {
            //            MessageBox.Show($"Не удалось удалить категорию '{categoryToDelete.Name}'",
            //                           "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        }
            //    }

            //    MessageBox.Show("Изменения сохранены успешно!",
            //                   "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            //    // Обновляем данные
            //    LoadCategories();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Ошибка сохранения: {ex.Message}\n{ex.StackTrace}",
            //                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
