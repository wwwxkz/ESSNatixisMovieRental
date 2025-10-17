using MovieRental.Domain.Entities;

namespace MovieRental.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default);
    }
}
