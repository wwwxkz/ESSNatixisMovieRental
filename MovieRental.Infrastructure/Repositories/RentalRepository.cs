using Microsoft.EntityFrameworkCore;
using MovieRental.Infrastructure.Data;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;

namespace MovieRental.Infrastructure.Repositories
{
    public class RentalRepository : Repository<Rental>, IRentalRepository
    {
        public RentalRepository(MovieRentalDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Rental>> GetRentalsByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Movie)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.ReturnDate == null)
                .Include(r => r.Movie)
                .Include(r => r.Customer)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasActiveRentalAsync(int customerId, int movieId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(r => 
                r.CustomerId == customerId && 
                r.MovieId == movieId && 
                r.ReturnDate == null, 
                cancellationToken);
        }

        public async Task<bool> ReturnRentalAsync(int rentalId, CancellationToken cancellationToken = default)
        {
            var rental = await _dbSet.FindAsync(new object[] { rentalId }, cancellationToken);
            if (rental == null || rental.ReturnDate.HasValue)
                return false;

            rental.ReturnDate = DateTime.UtcNow;
            _context.Entry(rental).State = EntityState.Modified;
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.Customer.Name.Contains(customerName))
                .Include(r => r.Movie)
                .Include(r => r.Customer)
                .ToListAsync(cancellationToken);
        }
    }
}
