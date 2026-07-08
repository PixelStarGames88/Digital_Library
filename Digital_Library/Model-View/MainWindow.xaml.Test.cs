using Digital_Library.Models.DataBaseConnector;
using Digital_Library.Models.DataBaseEntities;
using System.Windows;

namespace Digital_Library;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private void TestMethod()
    {
        DataBaseConnector connector = new DataBaseConnector();
        connector.Add(new Author { FirstName = "Jeffery", LastName = "Epstein" });
        connector.SaveChanges();
    }
}