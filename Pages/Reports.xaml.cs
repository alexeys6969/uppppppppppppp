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
    /// Логика взаимодействия для Reports.xaml
    /// </summary>
    public partial class Reports : Page
    {
        private Connection Connection;
        private string connect;
        private ObservableCollection<Report> _report;
        public Reports(string _connection)
        {
            InitializeComponent();
            Connection = new Connection();
            connect = _connection;
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                var loadedReports = Connection.GetReport(connect);

                _report = new ObservableCollection<Report>(loadedReports);
                reportsDataGrid.ItemsSource = _report;
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
                reportsDataGrid.CommitEdit();
                var currentReports = _report.ToList();
                var originalReports = Connection.GetReport(connect).ToList();

                foreach (var reports in currentReports)
                {
                    if (reports.Id > 0) // Существующая запись
                    {
                        var originalReport = originalReports.FirstOrDefault(c => c.Id == reports.Id);
                        if (originalReport != null &&
                            (originalReport.ReportType != reports.ReportType ||
                            originalReport.PeriodStart != reports.PeriodStart ||
                             originalReport.PeriodEnd != reports.PeriodEnd ||
                             originalReport.CreatedAt != reports.CreatedAt ||
                             originalReport.NameEmployee != reports.NameEmployee))
                        {
                            bool updated = Connection.UpdateReport(reports, connect);
                            if (!updated)
                            {
                                MessageBox.Show($"Не удалось обновить отчет '{reports.ReportType}'",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }

                foreach (var reports in currentReports.Where(c => c.Id == 0))
                {
                    int newId = Connection.AddReport(reports, connect);
                    if (newId > 0)
                    {
                        reports.Id = newId;
                    }
                }

                var reportsToDelete = originalReports.Where(oldRep =>
                    !currentReports.Any(newRep => newRep.Id == oldRep.Id)).ToList();

                foreach (var repToDelete in reportsToDelete)
                {
                    bool deleted = Connection.DeleteReport(repToDelete.Id, connect);
                    if (!deleted)
                    {
                        MessageBox.Show($"Не удалось удалить отчет '{repToDelete.ReportType}'",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Изменения сохранены успешно!",
                               "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}",
                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadReports();
            }
        }
    }
}
