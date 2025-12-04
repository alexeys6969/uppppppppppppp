using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для Category.xaml
    /// </summary>
    public partial class Category : Page
    {
        private Connection Connection;
        private string userRole;
        List<Models.Category> categories;
        Employees employee;
        public Category(string userPosition, Employees _employees)
        {
            InitializeComponent();
            Connection = new Connection();
            userRole = userPosition;
            LoadCategories();
            employee = _employees;
        }

        private void LoadCategories()
        {
            try
            {
                string roleConnection = Connection.GetConnection(userRole);
                categories = Connection.GetCategories(roleConnection);
                categoriesDataGrid.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем текущие данные из DataGrid
                categories = (List<Models.Category>)categoriesDataGrid.ItemsSource;

                // Собираем новые/измененные категории
                var newCategories = new List<Models.Category>();

                foreach (var category in categories)
                {
                    // Проверяем, новая ли это категория (например, по ID)
                    if (category.Id == 0) // 0 или -1 для новых записей
                    {
                        newCategories.Add(category);
                    }
                }

                // Добавляем новые категории
                foreach (var category in newCategories)
                {
                    Connection.AddCategory(category, userRole);
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем данные
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
