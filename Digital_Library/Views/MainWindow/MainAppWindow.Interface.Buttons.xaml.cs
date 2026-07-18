using Digital_Library.Models.DataBaseConnector;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

public partial class MainAppWindow : Window
{
    private void AddButtnonCreate(StackPanel panel)
    {
        Button btnAdd = new Button { Content = "Добавить", Width = 100, Margin = new Thickness(5) };
        btnAdd.Click += (s, e) =>
        {
            AddWindow addWin = new AddWindow();
            if (addWin.ShowDialog() == true) RefreshAllTabs();
        };
        panel.Children.Add(btnAdd);
    }
    private void AddButtnonDelete(StackPanel panel, RoutedEventHandler fun)
    {
        Button btnDel = new Button { Content = "Удалить", Margin = new Thickness(5), Width = 95 };
        btnDel.Click += fun;
        panel.Children.Add(btnDel);
    }
    private void AddButtnonUpdate(StackPanel panel, RoutedEventHandler fun)
    {
        Button btnSave = new Button { Content = "Сохранить", Margin = new Thickness(5), Width = 110 };
        btnSave.Click += fun; 
        panel.Children.Add(btnSave);
    }

    private void AddButtonDetails(StackPanel panel, DataGrid dg, string tabName)
    {
        Button btnDetails = new Button { Content = "Подробнее", Margin = new Thickness(5), Width = 110 };

        btnDetails.Click += (s, e) =>
        {
            var selectedItem = dg.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Сначала выберите запись в таблице.");
                return;
            }
            if (tabName == "Издательство")
            {
                string publisherName = (selectedItem as dynamic).Name;
                if (_publisherDescriptions.TryGetValue(publisherName, out string desc))
                    MessageBox.Show(desc, publisherName);
                else
                    MessageBox.Show("Описание для данного издательства пока отсутствует.", publisherName);
            }

            else if (tabName == "Обзор")
            {
                string description = 
                "Название: " + (selectedItem as dynamic).Title + 
                "\nАвторы: " + (selectedItem as dynamic).Authors + 
                "\nИздатель: " + (selectedItem as dynamic).PublisherName +
                "\nГод издания: " + (selectedItem as dynamic).PublicationYear +
                "\nКоличество страниц: " + (selectedItem as dynamic).PageCount + " c." +
                "\nКлючевые слова: " + (selectedItem as dynamic).Keywords +
                "\nАннотации: " + (selectedItem as dynamic).Annotation;
                MessageBox.Show(description, "Описание");
            }
        };

        panel.Children.Add(btnDetails);
    }
}