using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using Digital_Library.Models.DataBaseConnector;
using Digital_Library.Models.DataBaseEntities;

namespace Digital_Library
{
    public partial class AddWindow : Window
    {
        private DataBaseConnector _db = new DataBaseConnector();
        public ObservableCollection<Author> AuthorsList { get; set; } = new ObservableCollection<Author>();

        public AddWindow()
        {
            InitializeComponent();
            dgAuthors.ItemsSource = AuthorsList;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtYear.Text))
            {
                MessageBox.Show("Название и год обязательны.");
                return;
            }

            if (!int.TryParse(txtYear.Text, out int year) || year < 0 || year > 2026)
            {
                MessageBox.Show("Введите корректный год.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtIsbn.Text) && !txtIsbn.Text.All(c => char.IsDigit(c) || c == '-'))
            {
                MessageBox.Show("ISBN может содержать только цифры и дефисы.");
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

                if (author.LastName.Any(char.IsDigit))
                {
                    MessageBox.Show($"Фамилия и инициалы '{author.LastName}' не должны содержать цифры.");
                    return;
                }
            }

            var publisher = _db.Publishers.FirstOrDefault(p => p.Name == txtPublisher.Text.Trim())
                            ?? new Publisher { Name = txtPublisher.Text.Trim() };

            var newPub = new Publication
            {
                Title = txtTitle.Text,
                PublicationYear = year,
                Isbn = txtIsbn.Text,
                Publisher = publisher
            };

            foreach (var authorRow in AuthorsList)
            {
                if (string.IsNullOrWhiteSpace(authorRow.LastName)) continue;
                var author = _db.Authors.FirstOrDefault(a => a.FirstName == authorRow.FirstName && a.LastName == authorRow.LastName) ?? authorRow;
                newPub.PublicationAuthors.Add(new PublicationAuthor { Author = author });
            }

            _db.Publications.Add(newPub);
            _db.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}