using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
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
        private string connect;
        private ObservableCollection<Models.Category> categories; 
        public Category(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var loadedCategories = Connection.GetCategories(connect);

                categories = new ObservableCollection<Models.Category>(loadedCategories);
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
            MainWindow.mainWindow.frame.Navigate(new Pages.Navigation(connect));
        }

        private void AcceptChange(object sender, RoutedEventArgs e)
        {
            try
            {
                categoriesDataGrid.CommitEdit();
                var currentCategories = categories.ToList();
                var originalCategories = Connection.GetCategories(connect).ToList();

                foreach (var category in currentCategories)
                {
                    if (category.Id > 0) // Существующая запись
                    {
                        var originalCategory = originalCategories.FirstOrDefault(c => c.Id == category.Id);
                        if (originalCategory != null &&
                            (originalCategory.Name != category.Name ||
                             originalCategory.Description != category.Description))
                        {
                            bool updated = Connection.UpdateCategory(category, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить категорию '{category.Name}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                // 2. ДОБАВЛЕНИЕ новых категорий
                foreach (var category in currentCategories.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddCategory(category, connect);
                    if (newId > 0)
                    {
                        category.Id = newId;
                    }
                }

                // 3. УДАЛЕНИЕ категорий
                var categoriesToDelete = originalCategories.Where(oldCat =>
                    !currentCategories.Any(newCat => newCat.Id == oldCat.Id)).ToList();

                foreach (var categoryToDelete in categoriesToDelete)
                {
                    bool deleted = Connection.DeleteCategory(categoryToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить категорию '{categoryToDelete.Name}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
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
                LoadCategories();
            }
        }
    }
}
