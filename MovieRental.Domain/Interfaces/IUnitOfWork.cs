namespace MovieRental.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }
        ICustomerRepository Customers { get; }
        IRentalRepository Rentals { get; }
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
