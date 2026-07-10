using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Models.DataBaseEntities;

[Table("publicationauthor", Schema = "public")]
public class PublicationAuthor
{
    [Column("publication_id")]
    public int PublicationId { get; set; }
    [Column("author_id")]
    public int AuthorId { get; set; }

    [ForeignKey("PublicationId")]
    public virtual Publication Publication { get; set; } = null!;

    [ForeignKey("AuthorId")]
    public virtual Author Author { get; set; } = null!;
}