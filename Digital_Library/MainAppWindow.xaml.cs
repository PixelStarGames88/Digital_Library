using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Digital_Library.Models.DataBaseConnector;
using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;

namespace Digital_Library
{
    public class PublicationViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;
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

        private bool UserHasPermission(string tableName, char requiredPermission)
        {
            if (!_permissions.ContainsKey(tableName)) return false;

            return _permissions[tableName].Contains(requiredPermission.ToString());
        }

        private FrameworkElement CreateOverviewTab()
        {
            DockPanel panel = new DockPanel();

            DataGrid dg = new DataGrid { IsReadOnly = true, AutoGenerateColumns = false };

            dg.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Title"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Авторы", Binding = new System.Windows.Data.Binding("Authors"), Width = new DataGridLength(1.5, DataGridLengthUnitType.Star) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Издательство", Binding = new System.Windows.Data.Binding("PublisherName"), Width = DataGridLength.Auto });
            dg.Columns.Add(new DataGridTextColumn { Header = "Год", Binding = new System.Windows.Data.Binding("PublicationYear"), Width = DataGridLength.Auto });

            dg.ItemsSource = LoadOverviewData();

            if (UserHasPermission("publication", 'C')) 
            {
                Button btnAdd = new Button { Content = "Добавить", Width = 100, Margin = new Thickness(5) };
                btnAdd.Click += (s, e) => {
                    AddWindow addWin = new AddWindow();
                    if (addWin.ShowDialog() == true) RefreshAllTabs();
                };
                DockPanel.SetDock(btnAdd, Dock.Top);
                panel.Children.Add(btnAdd);
            }

            panel.Children.Add(dg);
            return panel;

            return panel;
        }

        private List<PublicationViewModel> LoadOverviewData()
        {
            return _db.Publications
                .Include(p => p.Publisher)
                .Include(p => p.PublicationAuthors).ThenInclude(pa => pa.Author)
                .Select(p => new PublicationViewModel
                {
                    Title = p.Title,
                    Authors = string.Join(", ", p.PublicationAuthors.Select(pa => pa.Author.FirstName + " " + pa.Author.LastName)),
                    PublisherName = p.Publisher != null ? p.Publisher.Name : "Нет издателя",
                    PublicationYear = p.PublicationYear
                }).ToList();
        }

        private FrameworkElement CreateTabContent(string tableName, string p)
        {
            DockPanel panel = new DockPanel();
            StackPanel btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Height = 40 };

            DataGrid dg = new DataGrid { IsReadOnly = !p.Contains("U"), AutoGenerateColumns = false };

            dg.CellEditEnding += (s, args) =>
            {
                var editedColumn = args.Column.Header.ToString();
                var editedValue = (args.EditingElement as TextBox)?.Text?.Trim();

                bool isInvalid = false;

                if (editedColumn == "Год")
                {
                    if (!int.TryParse(editedValue, out int y) || y < 0 || y > 2026)
                        isInvalid = true;
                }
                else if (editedColumn == "ISBN")
                {
                    if (!string.IsNullOrEmpty(editedValue) && !editedValue.All(c => char.IsDigit(c) || c == '-'))
                        isInvalid = true;
                }
                else if (editedColumn == "Фамилия" || editedColumn == "Инициалы" || editedColumn == "Издательство")
                {
                    if (editedValue.Any(char.IsDigit))
                        isInvalid = true;
                }

                if (isInvalid)
                {
                    MessageBox.Show($"Ошибка в поле '{editedColumn}': введены недопустимые данные.");
                    args.Cancel = true; 
                }
            };

            if (tableName == "author")
            {
                dg.Columns.Add(new DataGridTextColumn { Header = "Инициалы", Binding = new System.Windows.Data.Binding("FirstName") });
                dg.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new System.Windows.Data.Binding("LastName") });
            }
            else if (tableName == "publisher")
            {
                dg.Columns.Add(new DataGridTextColumn { Header = "Издательство", Binding = new System.Windows.Data.Binding("Name") });
            }
            else if (tableName == "publication")
            {
                dg.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new System.Windows.Data.Binding("Title") });
                dg.Columns.Add(new DataGridTextColumn { Header = "Год", Binding = new System.Windows.Data.Binding("PublicationYear") });
                dg.Columns.Add(new DataGridTextColumn { Header = "ISBN", Binding = new System.Windows.Data.Binding("Isbn") });
                dg.Columns.Add(new DataGridTextColumn { Header = "Издательство", Binding = new System.Windows.Data.Binding("Publisher.Name") });
            }

            if (p.Contains("U"))
            {
                Button btnSave = new Button { Content = "Сохранить", Margin = new Thickness(5), Width = 110 };
                btnSave.Click += (s, e) => {
                    try
                    {
                        _db.SaveChanges();
                        MessageBox.Show("Изменения сохранены.");
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка сохранения. Проверьте введенные данные.");
                    }
                };
                btnPanel.Children.Add(btnSave);
            }

            if (p.Contains("D"))
            {
                Button btnDel = new Button { Content = "Удалить", Margin = new Thickness(5), Width = 95 };
                btnDel.Click += (s, e) => {
                    if (dg.SelectedItem != null)
                    {
                        _db.Remove(dg.SelectedItem);
                        _db.SaveChanges();
                        RefreshData(dg, tableName);
                    }
                };
                btnPanel.Children.Add(btnDel);
            }

            DockPanel.SetDock(btnPanel, Dock.Top);
            panel.Children.Add(btnPanel);
            panel.Children.Add(dg);
            RefreshData(dg, tableName);
            return panel;
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
            var dg = (tab?.Content as DockPanel)?.Children.OfType<DataGrid>().FirstOrDefault()
                     ?? (tab?.Content as DataGrid); 

            if (dg == null) return;

            string filter = searchText.ToLower();
            string header = tab.Header?.ToString() ?? "";

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

        private void Find_Click(object sender, RoutedEventArgs e) => PerformSearch(SearchBox.Text);

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Сохранить изменения перед выходом?", "Выход", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Yes)
            {
                _db.SaveChanges();
                ProceedToLogout();
            }
            else if (result == MessageBoxResult.No)
            {
                ProceedToLogout();
            }
        }

        private void ProceedToLogout()
        {
            new MainWindow().Show();
            this.Close();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PerformSearch(SearchBox.Text);
        }

        private void RefreshAllTabs()
        {
            foreach (var item in MainTabs.Items)
            {
                if (item is TabItem tab)
                {
                    var dg = (tab.Content as DockPanel)?.Children.OfType<DataGrid>().FirstOrDefault()
                             ?? (tab.Content as DataGrid);

                    if (dg != null)
                    {
                        string header = tab.Header.ToString();
                        if (header == "Обзор")
                        {
                            dg.ItemsSource = LoadOverviewData();
                        }
                        else
                        {
                            RefreshData(dg, header);
                        }
                    }
                }
            }
        }
    }
}