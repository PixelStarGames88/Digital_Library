using Digital_Library.Models.DataBaseConnector;
using Digital_Library.Models.DataBaseEntities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Digital_Library;

public partial class AddWindow : Window
{
    private DataBaseConnector _db = new DataBaseConnector();
    public ObservableCollection<Author> AuthorsList { get; set; } = new ObservableCollection<Author>();

    public AddWindow()
    {
        InitializeComponent();
        dgAuthors.ItemsSource = AuthorsList;
    }

    
}