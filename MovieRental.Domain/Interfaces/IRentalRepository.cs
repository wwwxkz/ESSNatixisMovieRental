using MovieRental.Domain.Entities;

namespace MovieRental.Domain.Interfaces
{
    public interface IRentalRepository : IRepository<Rental>
    {
        Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default);
        Task<IEnumerable<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken = default);
        Task<bool> HasActiveRentalAsync(int customerId, int movieId, CancellationToken cancellationToken = default);
        Task<bool> ReturnRentalAsync(int rentalId, CancellationToken cancellationToken = default);
    }
}
