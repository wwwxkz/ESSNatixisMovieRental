using System.ComponentModel.DataAnnotations;

namespace MovieRental.Domain.DTOs;

public class CreateRentalDto
{
    [Required]
    public int MovieId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    [Range(1, 30, ErrorMessage = "Rental period must be between 1 and 30 days.")]
    public int DaysRented { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Payment method must be between 3 and 20 characters.")]
    public string PaymentMethod { get; set; } = "CreditCard";
}
