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
    public int? PublicationYear { get; set; }
    public int PageCount { get; set; }
    public string Annotation { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}

public partial class MainAppWindow : Window
{
    private DataBaseConnector _db = new DataBaseConnector();
    private Dictionary<string, string> _permissions;

    public Dictionary<string, string> _publisherDescriptions = new Dictionary<string, string>
{
    { "ЛТИ им. Ленсовета", "Ленинградский технологический институт им. Ленсовета (на сегодняшний день СПБГТИ (ТУ)). Ведущий вуз в области химии и технологий." },
    { "СПб университет", "Санкт-Петербургский государственный университет — старейший вуз России, центр передовой науки и высшего образования с почти трехсотлетней историей." },
    { "СПбТИ(ТУ)", "Санкт-Петербургский государственный технологический институт (технический университет) - ведущий российский вуз в области химии, химической технологии, биотехнологии, нанотехнологии, механики, информационных технологий, управления и экономики." }
};
    public MainAppWindow(string role, Dictionary<string, string> permissions)
    {
        InitializeComponent();
        _permissions = permissions;
        MainTabs.Items.Clear();
        var tabNames = new Dictionary<string, string>
    {
        { "author", "Автор" },
        { "publication", "Публикация" },
        { "publisher", "Издательство" }
    };

        TabItem overviewTab = new TabItem { Header = "Обзор" };
        overviewTab.Content = CreateOverviewTab();
        MainTabs.Items.Add(overviewTab);

        foreach (var table in _permissions.Keys)
        {
            string displayName = tabNames.ContainsKey(table) ? tabNames[table] : table;

            TabItem tab = new TabItem { Header = displayName };
            tab.Content = CreateTabContent(table, _permissions[table]);
            MainTabs.Items.Add(tab);
        }
    }
}