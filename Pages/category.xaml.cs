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
            MainWindow.mainWindow.frame.
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем текущие данные из DataGrid
                var currentCategories = (List<Models.Category>)categoriesDataGrid.ItemsSource;

                // Сначала обновляем существующие записи
                foreach (var category in currentCategories)
                {
                    if (category.Id > 0) // Если ID > 0, значит существующая запись
                    {
                        // Проверяем, изменилась ли категория
                        var originalCategory = categories.FirstOrDefault(c => c.Id == category.Id);
                        if (originalCategory != null &&
                            (originalCategory.Name != category.Name ||
                             originalCategory.Description != category.Description))
                        {
                            Connection.UpdateCategory(category, userRole);
                        }
                    }
                }

                // Затем добавляем новые категории
                foreach (var category in currentCategories)
                {
                    if (category.Id == 0) // Если ID == 0, значит новая запись
                    {
                        int newId = Connection.AddCategory(category, userRole);
                        category.Id = newId; // Обновляем ID в объекте
                    }
                }

                // Удаляем категории, которые были удалены из DataGrid
                var deletedCategories = categories.Where(oldCat =>
                    !currentCategories.Any(newCat => newCat.Id == oldCat.Id));

                foreach (var deletedCategory in deletedCategories)
                {
                    Connection.DeleteCategory(deletedCategory.Id, userRole);
                }

                // Обновляем исходный список
                categories = currentCategories.ToList();

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновляем данные (если нужно)
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
