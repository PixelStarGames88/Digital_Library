using Digital_Library.Models.DataBaseConnector;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

public partial class MainAppWindow : Window
{

    private FrameworkElement CreateOverviewTab()
    {
        DockPanel panel = new DockPanel();
        StackPanel btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Height = 40 };
        DataGrid dg = AddOverviewTab(); 

        DockPanel.SetDock(btnPanel, Dock.Top);
        panel.Children.Add(btnPanel);
        panel.Children.Add(dg);

        AddButtonDetails(btnPanel, dg, "Обзор");

        if (UserHasPermission("publication", 'C')) AddButtnonCreate(btnPanel);
        if (UserHasPermission("publication", 'U')) AddButtnonUpdate(btnPanel, (s, e) =>
        {
            try
            {
                AuthorException(dg);
                PublisherChange(dg);
                RefreshAllTabs();
                MessageBox.Show("Изменения сохранены.");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения. Проверьте введенные данные.");
            }
        });

        return panel;
    }

    private DataGrid AddOverviewTab()
    {
        DataGrid dg = new DataGrid { IsReadOnly = !UserHasPermission("publication", 'U'), AutoGenerateColumns = false };

        dg.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Title"), Width = new DataGridLength(2, DataGridLengthUnitType.Star), IsReadOnly = true});
        dg.Columns.Add(new DataGridTextColumn { Header = "Автор", Binding = new System.Windows.Data.Binding("Authors"), Width = new DataGridLength(1.5, DataGridLengthUnitType.Star) });
        dg.Columns.Add(new DataGridTextColumn { Header = "Издательство", Binding = new System.Windows.Data.Binding("PublisherName"), Width = DataGridLength.Auto});
        dg.Columns.Add(new DataGridTextColumn { Header = "Год", Binding = new System.Windows.Data.Binding("PublicationYear"), Width = DataGridLength.Auto, IsReadOnly = true });

        dg.ItemsSource = LoadOverviewData();

        return dg;

    }
    
    private FrameworkElement CreateTabContent(string tableName, string p)
    {
        DockPanel panel = new DockPanel();
        StackPanel btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Height = 40 };
        DataGrid dg;

        if (tableName == "author")
        {
            dg = AddAuthorTab(p);
        }   
        else if (tableName == "publisher")
        {
            dg = AddPublisherTab(p);
            AddButtonDetails(btnPanel, dg, "Издательство");
        }
        else if (tableName == "publication")
        {
            dg = AddPublicationTab(p);
        }  
        else
        {
            dg = AddOverviewTab();
            AddButtonDetails(btnPanel, dg, "Обзор");
        }
            

        if (p.Contains("U"))
            AddButtnonUpdate(btnPanel, (s, e) =>
            {
                try
                {
                    _db.SaveChanges();
                    RefreshAllTabs();
                    MessageBox.Show("Изменения сохранены.");
                }
                catch
                {
                    MessageBox.Show("Ошибка сохранения. Проверьте введенные данные.");
                }
            });

        if (p.Contains("D"))
            AddButtnonDelete(btnPanel, (o, e) => 
            {
                if (dg.SelectedItem != null)
                {
                    _db.Remove(dg.SelectedItem);
                    _db.SaveChanges();
                    RefreshAllTabs();
                }
            });

        DockPanel.SetDock(btnPanel, Dock.Top);
        panel.Children.Add(btnPanel);
        
        panel.Children.Add(dg);
        RefreshData(dg, tableName);
        return panel;
    }
    private DataGrid AddAuthorTab(string p)
    {
        DataGrid dg = new DataGrid { IsReadOnly = !p.Contains("U"), AutoGenerateColumns = false };

        dg.CellEditEnding += EditEnding;

        dg.Columns.Add(new DataGridTextColumn { Header = "Инициалы", Binding = new System.Windows.Data.Binding("FirstName") });
        dg.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new System.Windows.Data.Binding("LastName") });

        return dg;
    }
    private DataGrid AddPublisherTab(string p)
    {
        DataGrid dg = new DataGrid { IsReadOnly = !p.Contains("U"), AutoGenerateColumns = false };

        dg.CellEditEnding += EditEnding;

        dg.Columns.Add(new DataGridTextColumn { Header = "Издательство", Binding = new System.Windows.Data.Binding("Name") });

        return dg;
    }
    private DataGrid AddPublicationTab(string p)
    {
        DataGrid dg = new DataGrid { IsReadOnly = !p.Contains("U"), AutoGenerateColumns = false };

        dg.CellEditEnding += EditEnding;

        dg.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Title"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
        dg.Columns.Add(new DataGridTextColumn { Header = "Год", Binding = new System.Windows.Data.Binding("PublicationYear") }); 
        dg.Columns.Add(new DataGridTextColumn { Header = "Ключевые слова", Binding = new System.Windows.Data.Binding("Keywords")});
        dg.Columns.Add(new DataGridTextColumn { Header = "Аннотация", Binding = new System.Windows.Data.Binding("Annotation")});
        dg.Columns.Add(new DataGridTextColumn { Header = "Количество страниц", Binding = new System.Windows.Data.Binding("PageCount")});


        return dg;
    }
    private void EditEnding(object? s, DataGridCellEditEndingEventArgs args)
    {
        var editedColumn = args.Column.Header.ToString();
        var editedValue = (args.EditingElement as TextBox)?.Text?.Trim();

        bool isInvalid = false;

        if (editedColumn == "Год")
        {
            if (!int.TryParse(editedValue, out int y) || y < 0 || y > 2026)
                isInvalid = true;
        }
        else if (editedColumn == "Количество страниц")
        {
            if (!int.TryParse(editedValue, out int p) || p <= 0)
                isInvalid = true;
        }
        else if (editedColumn == "Ключевые слова" || editedColumn == "Аннотация")
        {
            if (string.IsNullOrWhiteSpace(editedValue))
                isInvalid = true;
        }
        else if (editedColumn == "Фамилия" || editedColumn == "Инициалы" || editedColumn == "Издательство")
        {
            if (string.IsNullOrEmpty(editedValue) || editedValue.Any(char.IsDigit))
                isInvalid = true;
        }
        if (isInvalid)
        {
            MessageBox.Show($"Ошибка в поле '{editedColumn}': введены недопустимые данные.");
            args.Cancel = true;
        }
    }
}
