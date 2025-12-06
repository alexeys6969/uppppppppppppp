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
        private string connect;
        private ObservableCollection<Employees> _employees; 
        public employees(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadEmployees();
        }
        private void LoadEmployees()
        {
            try
            {
                var loadedEmployees = Connection.GetEmployees(connect);

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
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(connect));
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            try
            {
                employeesDataGrid.CommitEdit();
                var currentEmployees = _employees.ToList();
                var originalEmployees = Connection.GetEmployees(connect).ToList();

                foreach (var employees in currentEmployees)
                {
                    if (employees.employee_id > 0) // Существующая запись
                    {
                        var originalEmployee = originalEmployees.FirstOrDefault(c => c.employee_id == employees.employee_id);
                        if (originalEmployee != null &&
                            (originalEmployee.full_name != employees.full_name ||
                             originalEmployee.position != employees.position))
                        {
                            bool updated = Connection.UpdateEmployee(employees, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить категорию '{employees.full_name}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var employees in currentEmployees.Where(c => c.employee_id == 0))
                {
                    int newId = Connection.AddEmployees(employees, connect);
                    if (newId > 0)
                    {
                        employees.employee_id = newId;
                    }
                }

                var employeesToDelete = originalEmployees.Where(oldEmp =>
                    !currentEmployees.Any(newEmp => newEmp.employee_id == oldEmp.employee_id)).ToList();

                foreach (var emplToDelete in employeesToDelete)
                {
                    bool deleted = Connection.DeleteEmployees(emplToDelete.employee_id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить сотрудника '{emplToDelete.full_name}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadEmployees();
            }
        }
    }
}
