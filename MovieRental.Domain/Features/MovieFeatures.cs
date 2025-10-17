using Microsoft.Extensions.Logging;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MovieRental.Domain.Features;

public class MovieFeatures : IMovieFeatures
{
    private readonly IMovieRepository _movieRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MovieFeatures> _logger;

    public MovieFeatures(
        IMovieRepository movieRepository,
        IUnitOfWork unitOfWork,
        ILogger<MovieFeatures> logger)
    {
        _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Movie> SaveAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationContext = new ValidationContext(movie);
            Validator.ValidateObject(movie, validationContext, validateAllProperties: true);

            _logger.LogInformation("Saving movie: {MovieTitle}", movie.Title);
            
            await _movieRepository.AddAsync(movie, cancellationToken);
            
            await _unitOfWork.CompleteAsync(cancellationToken);
            return movie;
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error while saving movie: {Message}", ex.Message);
            throw new InvalidOperationException("Invalid movie data: " + ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving movie");
            throw;
        }
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all movies");
        var movies = await _movieRepository.GetAllAsync(cancellationToken);
        return movies.OrderBy(m => m.Title);
    }

    public async Task<Movie?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting movie by ID: {MovieId}", id);
        return await _movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Movie> AddAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new movie: {MovieTitle}", movie.Title);
            
            var validationContext = new ValidationContext(movie);
            Validator.ValidateObject(movie, validationContext, validateAllProperties: true);
            
            await _movieRepository.AddAsync(movie, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            
            _logger.LogInformation("Successfully added movie with ID: {MovieId}", movie.Id);
            return movie;
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error while adding movie: {Message}", ex.Message);
            throw new InvalidOperationException($"Invalid movie data: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding movie");
            throw;
        }
    }
}
