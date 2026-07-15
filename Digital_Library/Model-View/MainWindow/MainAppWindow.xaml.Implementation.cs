using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

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
        if (tableName == "publication") dg.ItemsSource = _db.Publications.ToList();
        else if (tableName == "author") dg.ItemsSource = _db.Authors.ToList();
        else if (tableName == "publisher") dg.ItemsSource = _db.Publishers.ToList();
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
        // Логика экспорта отфильтрованных данных 
    }

    // Универсальный метод для всех элементов поиска/фильтрации
    private void Filter_Changed(object sender, System.EventArgs e)
    {
        // Универсальный метод для всех элементов поиска/фильтрации
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
