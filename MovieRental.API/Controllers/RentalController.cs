using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieRental.Domain.DTOs;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MovieRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalFeatures _rentalFeatures;
        private readonly ILogger<RentalController> _logger;

        public RentalController(
            IRentalFeatures rentalFeatures,
            ILogger<RentalController> logger)
        {
            _rentalFeatures = rentalFeatures ?? throw new ArgumentNullException(nameof(rentalFeatures));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets rentals by customer name (partial match)
        /// </summary>
        [HttpGet("customer/{customerName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRentalsByCustomerName(string customerName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return BadRequest("Customer name cannot be empty");
            }

            try
            {
                var rentals = await _rentalFeatures.GetRentalsByCustomerNameAsync(customerName, cancellationToken);
                return Ok(rentals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rentals for customer: {CustomerName}", customerName);
                return StatusCode(500, "An error occurred while retrieving rentals.");
            }
        }

        /// <summary>
        /// Gets a rental by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var rental = await _rentalFeatures.GetByIdAsync(id, cancellationToken);
                if (rental == null)
                {
                    return NotFound();
                }
                return Ok(rental);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving rental with ID: {RentalId}", id);
                return StatusCode(500, "An error occurred while retrieving the rental.");
            }
        }

        /// <summary>
        /// Creates a new rental
        /// </summary>
        [HttpPost("rent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RentMovie([FromBody] CreateRentalDto rentalDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest(ModelState);
            }

            try
            {
                var rental = await _rentalFeatures.RentMovieAsync(rentalDto, cancellationToken);
                _logger.LogInformation("Created rental with ID {RentalId}", rental.Id);
                return CreatedAtAction(nameof(GetById), new { id = rental.Id }, rental);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating rental: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Create rental operation was cancelled");
                return StatusCode(StatusCodes.Status499ClientClosedRequest, "The request was cancelled by the client.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rental: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
