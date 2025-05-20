using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieRental.Rental
{
	public class Rental
	{
		[Key]
		public int Id { get; set; }
		public int DaysRented { get; set; }
		public Movie.Movie? Movie { get; set; }

		[ForeignKey("Movie")]
		public int MovieId { get; set; }

		public string PaymentMethod { get; set; }

		// TODO: we should have a table for the customers
		public string CustomerName { get; set; }
	}
}
