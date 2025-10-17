using Microsoft.EntityFrameworkCore;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;
using MovieRental.Infrastructure.Data;
using MovieRental.Infrastructure.Repositories;

namespace MovieRental.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MovieRentalDbContext _context;
        private IMovieRepository _movies;
        private ICustomerRepository _customers;
        private IRentalRepository _rentals;

        public UnitOfWork(MovieRentalDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IMovieRepository Movies => _movies ??= new MovieRepository(_context);
        public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
        public IRentalRepository Rentals => _rentals ??= new RentalRepository(_context);

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
