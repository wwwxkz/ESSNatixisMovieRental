namespace MovieRental.Rental;

public interface IRentalFeatures
{
	Rental Save(Rental rental);
	IEnumerable<Rental> GetRentalsByCustomerName(string customerName);
}