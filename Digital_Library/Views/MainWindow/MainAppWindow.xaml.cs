using Digital_Library.Models.DataBaseConnector;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

public class PublicationViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string PublisherName { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int? PublicationYear { get; set; }
}

public partial class MainAppWindow : Window
{
    private DataBaseConnector _db = new DataBaseConnector();
    private Dictionary<string, string> _permissions;

    public MainAppWindow(string role, Dictionary<string, string> permissions)
    {
        InitializeComponent();
        _permissions = permissions;

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
}