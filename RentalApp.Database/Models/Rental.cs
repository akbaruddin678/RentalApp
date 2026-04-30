using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentalApp.Database.Models;

public enum RentalStatus
{
    Requested,
    Approved,
    Rejected,
    OutForRent,
    Overdue,
    Returned,
    Completed
}

[Table("rentals")]
public class Rental
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public RentalStatus Status { get; set; } = RentalStatus.Requested;

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }

    [MaxLength(500)]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign keys
    public int ItemId { get; set; }
    public int BorrowerId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ItemId))]
    public Item Item { get; set; } = null!;

    [ForeignKey(nameof(BorrowerId))]
    public User Borrower { get; set; } = null!;

    public Review? Review { get; set; }
}
