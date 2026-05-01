using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RentalApp.Database.Models;
[Table("categories")]
public class Category
{
    public int Id { get; set; }
    [Required][MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(300)] public string Description { get; set; } = string.Empty;
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
