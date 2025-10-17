using MovieRental.Domain.Entities;

namespace MovieRental.Domain.Interfaces;

public interface IMovieFeatures
{
    /// <summary>
    /// Saves a movie to the database
    /// </summary>
    Task<Movie> SaveAsync(Movie movie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all movies ordered by title
    /// </summary>
    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a movie by its ID
    /// </summary>
    Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new movie to the database
    /// </summary>
    Task<Movie> AddAsync(Movie movie, CancellationToken cancellationToken = default);
}
