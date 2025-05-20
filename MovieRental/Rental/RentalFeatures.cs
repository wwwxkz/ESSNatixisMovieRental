using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		public RentalFeatures(MovieRentalDbContext movieRentalDb)
		{
			_movieRentalDb = movieRentalDb;
		}

		//TODO: make me async :(
		public Rental Save(Rental rental)
		{
			_movieRentalDb.Rentals.Add(rental);
			_movieRentalDb.SaveChanges();
			return rental;
		}

		//TODO: finish this method and create an endpoint for it
		public IEnumerable<Rental> GetRentalsByCustomerName(string customerName)
		{
			return [];
		}

	}
}
