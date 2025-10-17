using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using MovieRental.Domain.DTOs;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;
using MovieRental.Domain.PaymentProviders;

namespace MovieRental.Domain.Features;

public class RentalFeatures : IRentalFeatures
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RentalFeatures> _logger;
    private readonly IPaymentProviderFactory _paymentProviderFactory;

    public RentalFeatures(
        IRentalRepository rentalRepository,
        IMovieRepository movieRepository,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<RentalFeatures> logger,
        IPaymentProviderFactory paymentProviderFactory)
    {
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
        _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _paymentProviderFactory = paymentProviderFactory ?? throw new ArgumentNullException(nameof(paymentProviderFactory));
    }

    public async Task<Rental> SaveAsync(Rental rental, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving rental for customer {CustomerId} and movie {MovieId}", 
                rental.CustomerId, rental.MovieId);

            await _rentalRepository.AddAsync(rental, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return rental;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving rental");
            throw;
        }
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCustomerNameAsync(string customerName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting rentals for customer: {CustomerName}", customerName);
            var rentals = await _rentalRepository.GetRentalsByCustomerNameAsync(customerName, cancellationToken);
            return rentals;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rentals for customer: {CustomerName}", customerName);
            throw;
        }
    }

    public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting rental by ID: {RentalId}", id);
            return await _rentalRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rental with ID: {RentalId}", id);
            throw;
        }
    }

    public async Task<Rental> RentMovieAsync(CreateRentalDto rentalDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Renting movie {MovieId} for customer {CustomerId}", 
                rentalDto.MovieId, rentalDto.CustomerId);

            // Get movie and customer
            var movie = await _movieRepository.GetByIdAsync(rentalDto.MovieId, cancellationToken);
            if (movie == null)
            {
                throw new InvalidOperationException($"Movie with ID {rentalDto.MovieId} not found");
            }

            var customer = await _customerRepository.GetByIdAsync(rentalDto.CustomerId, cancellationToken);
            if (customer == null)
            {
                throw new InvalidOperationException($"Customer with ID {rentalDto.CustomerId} not found");
            }

            // Check if movie is available
            if (!movie.IsAvailable || movie.Stock <= 0)
            {
                throw new InvalidOperationException($"Movie '{movie.Title}' is not available for rent");
            }

            try
            {
                // Process payment
                var paymentProvider = _paymentProviderFactory.GetProvider(rentalDto.PaymentMethod);
                var paymentSucceeded = await paymentProvider.ProcessPaymentAsync(
                    movie.RentalPrice * rentalDto.DaysRented,
                    $"Rental for movie: {movie.Title}",
                    cancellationToken);

                if (!paymentSucceeded)
                {
                    throw new InvalidOperationException($"Payment failed using {rentalDto.PaymentMethod}");
                }

                // Create rental
                var rental = new Rental
                {
                    MovieId = rentalDto.MovieId,
                    CustomerId = rentalDto.CustomerId,
                    RentalDate = DateTime.UtcNow,
                    ReturnDate = null,
                    TotalCost = movie.RentalPrice * rentalDto.DaysRented,
                    DaysRented = rentalDto.DaysRented,
                    PaymentMethod = rentalDto.PaymentMethod
                };

                // Update movie stock
                movie.Stock--;
                if (movie.Stock == 0)
                {
                    movie.IsAvailable = false;
                }

                await _rentalRepository.AddAsync(rental, cancellationToken);
                await _movieRepository.UpdateAsync(movie, cancellationToken);
                
                await _unitOfWork.CompleteAsync(cancellationToken);
                return rental;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error processing rental");
                throw new InvalidOperationException("An error occurred while processing the rental.", ex);
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error renting movie {MovieId} for customer {CustomerId}", 
                rentalDto.MovieId, rentalDto.CustomerId);
            throw new InvalidOperationException("An error occurred while processing the rental.", ex);
        }
    }

    public async Task<Rental> ReturnMovieAsync(int rentalId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Returning movie for rental ID: {RentalId}", rentalId);
            
            var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken);
            if (rental == null)
            {
                throw new InvalidOperationException($"Rental with ID {rentalId} not found");
            }

            if (rental.ReturnDate.HasValue)
            {
                throw new InvalidOperationException("This movie has already been returned");
            }

            // Update rental
            rental.ReturnDate = DateTime.UtcNow;
            
            // Update movie stock
            var movie = await _movieRepository.GetByIdAsync(rental.MovieId, cancellationToken);
            if (movie != null)
            {
                movie.Stock++;
                if (!movie.IsAvailable)
                {
                    movie.IsAvailable = true;
                }
                await _movieRepository.UpdateAsync(movie, cancellationToken);
            }

            await _rentalRepository.UpdateAsync(rental, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            
            return rental;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error returning movie for rental ID: {RentalId}", rentalId);
            throw new InvalidOperationException("An error occurred while processing the return.", ex);
        }
    }
}
