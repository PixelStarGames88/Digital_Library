using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Digital_Library.Models.DataBaseEntities;

[Table("publisher", Schema = "public")]
public class Publisher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("publisher_id")]
    public int PublisherId { get; set; }
    [Column("name")]
    public string Name { get; set; } = null!;
}