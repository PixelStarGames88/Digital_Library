using Digital_Library.Models.DataBaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Digital_Library.Models.DataBaseConnector;

public class DataBaseConnector : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publication> Publications { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<PublicationAuthor> PublicationAuthors { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicationAuthor>().HasKey(pu => new { pu.PublicationId, pu.AuthorId });

        base.OnModelCreating(modelBuilder);
    }

}