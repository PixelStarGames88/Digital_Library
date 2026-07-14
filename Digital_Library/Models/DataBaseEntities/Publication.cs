using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Models.DataBaseEntities;

[Table("publication", Schema = "public")]
public class Publication
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("publication_id")] 
    public int PublicationId { get; set; }

    [Column("title")] 
    public string Title { get; set; } = null!;
    [Column("publication_year")] 
    public int? PublicationYear { get; set; }
    [Column("isbn")] 
    public string? Isbn { get; set; }
    
    [Column("publisher_id")] 
    public int PublisherId { get; set; }
    [Column("keywords")]
    public string? Keywords { get; set; }
    [Column("annotation")]
    public string? Annotation { get; set; }

    [Column("page_count")]
    public int PageCount { get; set; }

    [ForeignKey("PublisherId")] 
    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<PublicationAuthor> PublicationAuthors { get; set; } = new List<PublicationAuthor>();
}