using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Digital_Library;

public partial class MainAppWindow : Window
{
    private void PublisherChange(DataGrid dg)
    {
        foreach (var item in dg.Items)
        {
            PublicationViewModel? viewModel = null;

            if (item is PublicationViewModel vm)
                viewModel = vm;
            else if (item is DataRowView drv)
                viewModel = ItemFromDataRowView(drv);

            if (viewModel == null) return;


            AddingNewPublishers(viewModel);
        }
        
    }
    private void AddingNewPublishers(PublicationViewModel viewModel)
    {
        string publisher = viewModel.PublisherName;

        int publisherId = _db.Publishers.Where(s => s.Name == publisher).Select(s => s.PublisherId).FirstOrDefault();
        int publicationId = _db.Publications.Where(s => s.Title == viewModel.Title && s.PublicationYear == viewModel.PublicationYear).Select(s => s.PublicationId).FirstOrDefault();

        if (publisherId == 0)
        {
            _db.Publishers.Add(new Publisher {  Name = publisher });
            _db.SaveChanges();
            publisherId = _db.Publishers.Where(s => s.Name == viewModel.PublisherName).Select(s => s.PublisherId).FirstOrDefault();
        }

        Publication publication = _db.Publications.Where(p => p.PublicationId == publicationId).FirstOrDefault() ?? throw new NullReferenceException();
        publication.PublisherId = publisherId;
        _db.SaveChanges();
    }
}