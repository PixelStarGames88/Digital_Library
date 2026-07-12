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
        InitializeTabsComponent(_permissions);
    }  
}