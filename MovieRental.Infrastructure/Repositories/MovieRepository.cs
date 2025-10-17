using Microsoft.EntityFrameworkCore;
using MovieRental.Infrastructure.Data;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;

namespace MovieRental.Infrastructure.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(MovieRentalDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Movie>> GetAvailableMoviesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.IsAvailable && m.Stock > 0)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsMovieAvailableAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var movie = await GetByIdAsync(movieId, cancellationToken);
            return movie != null && movie.IsAvailable && movie.Stock > 0;
        }
    }
}
