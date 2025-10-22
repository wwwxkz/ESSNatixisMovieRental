using MovieRental.Domain.Entities;

namespace MovieRental.Domain.Interfaces;

public interface IMovieFeatures
{
    Task<Movie> SaveAsync(Movie movie, CancellationToken cancellationToken = default);

    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<Movie> AddAsync(Movie movie, CancellationToken cancellationToken = default);
}
