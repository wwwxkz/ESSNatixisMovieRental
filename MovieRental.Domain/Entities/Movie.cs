using System.ComponentModel.DataAnnotations;

namespace MovieRental.Domain.Entities
{
    public class Movie : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        [Range(0, double.MaxValue)]
        public decimal RentalPrice { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }
        
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
