using MovieRental.Domain.DTOs;
using MovieRental.Domain.Entities;

namespace MovieRental.Domain.Interfaces;

public interface IRentalFeatures
{
    /// <summary>
    /// Saves a rental to the database
    /// </summary>
    Task<Rental> SaveAsync(Rental rental, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets rentals by customer name (partial match)
    /// </summary>
    Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(
        string customerName, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a rental by its ID
    /// </summary>
    Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rents a movie for a customer
    /// </summary>
    Task<Rental> RentMovieAsync(CreateRentalDto rentalDto, CancellationToken cancellationToken = default);
}
