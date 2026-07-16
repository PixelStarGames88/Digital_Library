using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace Digital_Library;

public partial class MainAppWindow : Window
{
    private void InitializeTabsComponent(Dictionary<string, string> permissions)
    {
        TabItem overviewTab = new TabItem { Header = "Обзор" };
        overviewTab.Content = CreateOverviewTab();
        MainTabs.Items.Add(overviewTab);

        foreach (var table in permissions.Keys)
        {
            TabItem tab = new TabItem { Header = table };
            tab.Content = CreateTabContent(table, permissions[table]);
            MainTabs.Items.Add(tab);
        }
    }
    private bool UserHasPermission(string tableName, char requiredPermission)
    {
        if (!_permissions.ContainsKey(tableName)) return false;

        return _permissions[tableName].Contains(requiredPermission.ToString());
    }
    private void RefreshData(DataGrid dg, string tableName)
    {
        if (tableName == "Публикация") dg.ItemsSource = _db.Publications.ToList();
        else if (tableName == "Автор") dg.ItemsSource = _db.Authors.ToList();
        else if (tableName == "Издательство") dg.ItemsSource = _db.Publishers.ToList();
    }
    private void PerformSearch(string searchText)
    {
        var tab = MainTabs.SelectedItem as TabItem;
        var dg = (tab?.Content as DockPanel)?.Children.OfType<DataGrid>().FirstOrDefault() ?? (tab?.Content as DataGrid);

        if (dg == null) return;

        string filter = searchText.ToLower();
        string header = tab?.Header?.ToString() ?? "";

        if (header == "Обзор")
        {
            dg.ItemsSource = LoadOverviewData().Where(x =>
                x.Title.ToLower().Contains(filter) ||
                x.Authors.ToLower().Contains(filter)).ToList();
        }
        else
        {
            dg.ItemsSource = header switch
            {
                "publication" => _db.Publications.Where(p => p.Title.ToLower().Contains(filter)).ToList(),
                "author" => _db.Authors.Where(a => a.LastName.ToLower().Contains(filter)).ToList(),
                "publisher" => _db.Publishers.Where(p => p.Name.ToLower().Contains(filter)).ToList(),
                _ => dg.ItemsSource
            };
        }
    }


    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        ProceedToLogout();
    }

    private void ProceedToLogout()
    {
        new MainWindow().Show();
        this.Close();
    }

    private void BtnSaveResult_Click(object sender, RoutedEventArgs e)
    {
        var selectedTab = MainTabs.SelectedItem as TabItem;
        if (selectedTab == null)
        {
            MessageBox.Show("Не выбрана вкладка для сохранения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DataGrid dg = null!;
        if (selectedTab.Content is DockPanel dockPanel)
        {
            dg = dockPanel.Children.OfType<DataGrid>().FirstOrDefault() ?? throw new NullReferenceException();
        }
        else if (selectedTab.Content is DataGrid directDg)
        {
            dg = directDg;
        }

        if (dg == null || dg.ItemsSource == null)
        {
            MessageBox.Show("В текущей вкладке нет данных для сохранения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = "Результаты_библиотеки.txt",
            Title = "Сохранить результаты"
        };

        if (saveFileDialog.ShowDialog() != true) return;


        try
        {
            var lines = new List<string>();

            var headers = dg.Columns.Select(c => c.Header?.ToString() ?? "").ToList();
            lines.Add(string.Join(" | ", headers));
            lines.Add(new string('-', 50));

            foreach (var item in dg.ItemsSource)
            {
                if (item == null) continue;

                var rowValues = new List<string>();
                foreach (var column in dg.Columns)
                {
                    if (column is DataGridTextColumn textColumn && textColumn.Binding is System.Windows.Data.Binding binding)
                    {
                        string propertyName = binding.Path.Path;
                        var propertyInfo = item.GetType().GetProperty(propertyName);

                        string value = propertyInfo?.GetValue(item)?.ToString() ?? "";

                        value = value.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Trim();

                        rowValues.Add(value);
                    }
                    else
                    {
                        rowValues.Add("");
                    }
                    rowValues.Add("\n");
                }
                lines.Add(string.Join(" | ", rowValues));
            }

            File.WriteAllLines(saveFileDialog.FileName, lines, System.Text.Encoding.UTF8);

            MessageBox.Show($"Данные успешно сохранены в файл:\n{saveFileDialog.FileName}",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch
        {
            MessageBox.Show($"Произошла ошибка при сохранении файла!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void RefreshAllTabs()
    {
        foreach (var item in MainTabs.Items)
        {
            if (item is TabItem tab)
            {
                var dg = (tab.Content as DockPanel)?.Children.OfType<DataGrid>().FirstOrDefault()
                         ?? (tab.Content as DataGrid);

                if (dg == null) return;

                string header = tab.Header.ToString() ?? throw new NullReferenceException();
                if (header == "Обзор") dg.ItemsSource = LoadOverviewData();
                else RefreshData(dg, header);
            }
        }
    }
}
