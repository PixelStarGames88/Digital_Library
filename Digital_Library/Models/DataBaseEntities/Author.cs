using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Models.DataBaseEntities;

[Table("author", Schema = "public")]
public class Author
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("author_id")]
    public int AuthorId { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; } = null!;
    [Column("last_name")]
    public string LastName { get; set; } = null!;
}