using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

public partial class MainAppWindow : Window
{
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
                ISBN = p.Isbn ?? "",
                PublicationYear = p.PublicationYear
            }).ToList();
    }
    private void AuthorException(DataGrid dg)
    {
        foreach (var item in dg.Items)
        {
            PublicationViewModel? viewModel = null;

            if (item is PublicationViewModel vm)
                viewModel = vm;
            else if (item is DataRowView drv)
                viewModel = ItemFromDataRowView(drv);

            if (viewModel == null) return;

            var authors = _db.Publications
                .Join
                (
                    _db.PublicationAuthors, p => p.PublicationId,
                    pa => pa.PublicationId,
                    (p, pa) => new
                    {
                        publication = p,
                        publicationAuthor = pa
                    }
                ).Join
                (
                    _db.Authors, p => p.publicationAuthor.AuthorId,
                    a => a.AuthorId,
                    (p, a) => new
                    {
                        publicationName = p.publication.Title,
                        publicationYear = p.publication.PublicationYear,
                        publicationId = p.publication.PublicationId,
                        author = a
                    }
                ).Where(a => a.publicationName == viewModel.Title &&
                a.publicationYear == viewModel.PublicationYear).ToList();

            foreach (var author in authors)
            {
                if (!viewModel.Authors.Contains(author.author.FirstName + " " + author.author.LastName))
                {
                    var authorRegistration = _db.PublicationAuthors.Where(a => a.PublicationId == author.publicationId && a.AuthorId == author.author.AuthorId).ToList();
                    _db.PublicationAuthors.RemoveRange(authorRegistration);
                }
            }
            foreach (string author in viewModel.Authors.Split(','))
                AddingNewAuthors(author, viewModel);
        }
        _db.SaveChanges();
        RefreshAllTabs();
    }
    private void AddingNewAuthors(string author, PublicationViewModel viewModel)
    {
        if (_db.Authors.Select(s => (s.FirstName + " " + s.LastName) == author.Trim()).Any())
        {
            string cleanAuthor = author.Trim();

            int authorId = _db.Authors.Where(s => (s.FirstName + " " + s.LastName) == cleanAuthor).Select(s => s.AuthorId).FirstOrDefault();
            int publicationId = _db.Publications.Where(s => s.Title == viewModel.Title && s.PublicationYear == viewModel.PublicationYear).Select(s => s.PublicationId).FirstOrDefault();

            if (authorId == 0)
            {
                _db.Authors.Add(new Author { FirstName = cleanAuthor.Split(' ')[0], LastName = cleanAuthor.Split(' ')[1] });
                _db.SaveChanges();
                authorId = _db.Authors.Where(s => (s.FirstName + " " + s.LastName) == cleanAuthor).Select(s => s.AuthorId).FirstOrDefault();
            }

            if(!_db.PublicationAuthors.Where(pa => pa.AuthorId == authorId && pa.PublicationId == publicationId).Any())
                _db.PublicationAuthors.Add(new PublicationAuthor { AuthorId = authorId, PublicationId = publicationId });
        }
    }

    private PublicationViewModel ItemFromDataRowView(DataRowView drv)
    {
        string title = drv["Title"].ToString() ?? throw new NullReferenceException();
        int publicationYear = Convert.ToInt32(drv["PublicationYear"]);

        PublicationViewModel viewModel = _db.Publications
            .Include(p => p.Publisher)
            .Include(p => p.PublicationAuthors).ThenInclude(pa => pa.Author)
            .Where(p => p.Title == title && p.PublicationYear == publicationYear)
            .Select(p => new PublicationViewModel
            {
                Title = p.Title,
                Authors = string.Join(", ", p.PublicationAuthors.Select(pa => pa.Author.FirstName + " " + pa.Author.LastName)),
                PublisherName = p.Publisher != null ? p.Publisher.Name : "Нет издателя",
                ISBN = p.Isbn ?? "",
                PublicationYear = p.PublicationYear
            }).FirstOrDefault() ?? throw new NullReferenceException();
        return viewModel;
    }
}