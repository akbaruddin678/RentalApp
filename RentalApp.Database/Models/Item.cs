using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RentalApp.Database.Models;
[Table("items")]
public class Item
{
    public int Id { get; set; }
    [Required][MaxLength(200)] public string Title { get; set; } = string.Empty;
    [MaxLength(1000)] public string Description { get; set; } = string.Empty;
    [Column(TypeName = "decimal(10,2)")] public decimal DailyRate { get; set; }
    [MaxLength(500)] public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [MaxLength(300)] public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int OwnerId { get; set; }
    public int? CategoryId { get; set; }
    [ForeignKey(nameof(OwnerId))] public User Owner { get; set; } = null!;
    [ForeignKey(nameof(CategoryId))] public Category? Category { get; set; }
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
