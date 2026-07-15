using Digital_Library.Models.DataBaseConnector;
using Digital_Library.Models.DataBaseEntities;
using System.Collections.ObjectModel;
using System.Windows;

namespace Digital_Library;

public partial class AddWindow : Window
{
    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtYear.Text) || string.IsNullOrWhiteSpace(txtPages.Text))
        {
            MessageBox.Show("Название, год и количество страниц обязательны.");
            return;
        }

        if (!int.TryParse(txtYear.Text, out int year) || year < 0 || year > 2026)
        {
            MessageBox.Show("Введите корректный год.");
            return;
        }

        if (!int.TryParse(txtPages.Text, out int pages) || pages <= 0)
        {
            MessageBox.Show("Количество страниц должно быть положительным числом.");
            return;
        }

        if (txtPublisher.Text.Any(char.IsDigit) && !txtPublisher.Text.Any(char.IsLetter))
        {
            MessageBox.Show("Название издательства не может состоять только из цифр.");
            return;
        }

        if (AuthorsList.Count == 0)
        {
            MessageBox.Show("Добавьте хотя бы одного автора.");
            return;
        }

        foreach (var author in AuthorsList)
        {
            if (string.IsNullOrWhiteSpace(author.LastName) || string.IsNullOrWhiteSpace(author.FirstName))
            {
                MessageBox.Show("У всех авторов должны быть заполнены инициалы и фамилия.");
                return;
            }
        }

        var publisher = _db.Publishers.FirstOrDefault(p => p.Name == txtPublisher.Text.Trim())
                        ?? new Publisher { Name = txtPublisher.Text.Trim() };

        var newPub = new Publication
        {
            Title = txtTitle.Text,
            PublicationYear = year,
            PageCount = pages,             
            Annotation = txtAnnotation.Text, 
            Keywords = txtKeyWords.Text,    
            Publisher = publisher
        };

        foreach (var authorRow in AuthorsList)
        {
            var author = _db.Authors.FirstOrDefault(a => a.FirstName == authorRow.FirstName && a.LastName == authorRow.LastName)
                         ?? authorRow;
            newPub.PublicationAuthors.Add(new PublicationAuthor { Author = author });
        }

        _db.Publications.Add(newPub);
        _db.SaveChanges();
        DialogResult = true;
        Close();
    }
}
