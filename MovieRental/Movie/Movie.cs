using System.ComponentModel.DataAnnotations;

namespace MovieRental.Movie
{
	public class Movie
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }

	}
}
