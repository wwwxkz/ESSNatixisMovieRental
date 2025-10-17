using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MovieRental.Domain.Entities
{
    public class Customer : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        public string? PhoneNumber { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
