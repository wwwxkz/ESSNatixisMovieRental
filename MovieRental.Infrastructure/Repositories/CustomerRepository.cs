using Microsoft.EntityFrameworkCore;
using MovieRental.Infrastructure.Data;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;

namespace MovieRental.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(MovieRentalDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default)
        {
            return !await _dbSet
                .AnyAsync(c => c.Email == email && 
                             (excludeCustomerId == null || c.Id != excludeCustomerId), 
                             cancellationToken);
        }
    }
}
