using MovieRental.Domain.Entities;

namespace MovieRental.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<IEnumerable<Movie>> GetAvailableMoviesAsync(CancellationToken cancellationToken = default);
        Task<bool> IsMovieAvailableAsync(int movieId, CancellationToken cancellationToken = default);
    }
}
