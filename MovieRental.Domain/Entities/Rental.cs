using System;
using System.ComponentModel.DataAnnotations;

namespace MovieRental.Domain.Entities
{
    public class Rental : BaseEntity
    {
        [Required]
        public int MovieId { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public DateTime RentalDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReturnDate { get; set; }
        
        [Required, Range(1, 365)]
        public int DaysRented { get; set; }
        
        [Required, Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Movie Movie { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
    }
}
