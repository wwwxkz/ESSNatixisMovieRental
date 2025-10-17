using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieRental.Domain.DTOs;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Features;
using MovieRental.Domain.Interfaces;

namespace MovieRental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieFeatures _movieFeatures;
        private readonly ILogger<MovieController> _logger;

        public MovieController(
            IMovieFeatures movieFeatures,
            ILogger<MovieController> logger)
        {
            _movieFeatures = movieFeatures ?? throw new ArgumentNullException(nameof(movieFeatures));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Domain.Entities.Movie>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var movies = await _movieFeatures.GetAllAsync(cancellationToken);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movies");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving movies.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Domain.Entities.Movie>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var movie = await _movieFeatures.GetByIdAsync(id, cancellationToken);
                if (movie == null)
                {
                    return NotFound();
                }
                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie with ID {MovieId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the movie.");
            }
        }

        /// <summary>
        /// Creates a new movie
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Domain.Entities.Movie>> Create([FromBody] Domain.Entities.Movie movie, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return BadRequest(ModelState);
                }

                var createdMovie = await _movieFeatures.SaveAsync(movie, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = createdMovie.Id }, createdMovie);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating movie: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the movie.");
            }
        }
    }
}
