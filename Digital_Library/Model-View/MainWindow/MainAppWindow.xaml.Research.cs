using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

public partial class MainAppWindow : Window
{
    private void FillComboboxes()
    {
        FillAuthorsBox();
        FillYearBox();
    }
    private void FillAuthorsBox()
    {
        var authors = _db.Authors.Select(a => (a.FirstName + " " + a.LastName)).ToList();
        FilterAuthors.ItemsSource = authors;
    }
    private void FillYearBox()
    {
        var years = _db.Publications.Select(a => a.PublicationYear).Order().Distinct().ToList();
        FilterYears.ItemsSource = years;
    }
    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        Filter_Changed();
    }
    private void Filter_Changed(object sender, TextChangedEventArgs e)
    {
        Filter_Changed();
    }
    private void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ClearFilter();
        RefreshAllTabs();
    }
    private void ClearFilter()
    {
        FilterYears.SelectedItem = null;
        FilterAuthors.SelectedItem = null;
        FilterPageCount.Clear();
        NameSearchBox.Clear();
        WordsSearchBox.Clear();
    }
    private void Filter_Changed()
    {
         var tab = MainTabs.SelectedItem as TabItem;
        var dg = (tab?.Content as DockPanel)?.Children.OfType<DataGrid>().FirstOrDefault() ?? (tab?.Content as DataGrid);

        if (dg == null) return;

        string filterYear = FilterYears.SelectedItem?.ToString() ?? "";
        string filterAuthor = FilterAuthors.SelectedItem as string ?? "";
        string filterPageCount = FilterPageCount.Text;
        string nameFilter = NameSearchBox.Text;
        string wordsFilter = WordsSearchBox.Text;

        string header = tab?.Header?.ToString() ?? "";

        if (header == "Обзор")
        {
            dg.ItemsSource = LoadOverviewData().Where(x => x.Authors.Contains(filterAuthor) &&
            (x.Annotation.Contains(wordsFilter) || x.Keywords.Contains(wordsFilter)) &&
            x.Title.Contains(nameFilter) && x.PublicationYear.ToString()!.Contains(filterYear)
            && x.PageCount.ToString().Contains(filterPageCount)).ToList();
        }
        else
        {
            dg.ItemsSource = header switch
            {
                "Автор" => _db.Authors.Where(x => (x.FirstName + " " + x.LastName).Contains(filterAuthor)).ToList(),
                "Публикация" => _db.Publications.Where(x => ((x.Annotation!.Contains(wordsFilter) || x.Keywords!.Contains(wordsFilter)) &&
                                                x.Title.Contains(nameFilter) && x.PageCount.ToString().Contains(filterPageCount)) &&
                                                x.PublicationYear.ToString()!.Contains(filterYear)).ToList(),
                _ => dg.ItemsSource
            };
        }
    }
}