using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MovieRental.API.Controllers;
using MovieRental.Domain.DTOs;
using MovieRental.Domain.Entities;
using MovieRental.Domain.Features;
using MovieRental.Domain.Interfaces;
using Xunit;

namespace MovieRental.Tests.Controllers
{
    public class MovieControllerTests
    {
        private readonly Mock<IMovieFeatures> _mockMovieFeatures;
        private readonly Mock<ILogger<MovieController>> _mockLogger;
        private readonly MovieController _controller;

        public MovieControllerTests()
        {
            _mockMovieFeatures = new Mock<IMovieFeatures>();
            _mockLogger = new Mock<ILogger<MovieController>>();
            
            _controller = new MovieController(_mockMovieFeatures.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfMovies()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new() { Id = 1, Title = "Movie 1", IsAvailable = true, Stock = 5 },
                new() { Id = 2, Title = "Movie 2", IsAvailable = true, Stock = 3 }
            };

            _mockMovieFeatures
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(movies);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Movie>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoMoviesExist()
        {
            // Arrange
            _mockMovieFeatures
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Movie>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Movie>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetAll_ReturnsOnlyAvailableMovies_WhenFilterIsApplied()
        {
            // Arrange
            var movies = new List<Movie>
            {
                new() { Id = 1, Title = "Available Movie", IsAvailable = true, Stock = 5 },
                new() { Id = 2, Title = "Unavailable Movie", IsAvailable = false, Stock = 0 }
            };

            _mockMovieFeatures
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(movies.Where(m => m.IsAvailable).ToList());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Movie>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.True(returnValue[0].IsAvailable);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_ReturnsNotFound_WhenIdIsInvalid(int invalidId)
        {
            // Act
            var result = await _controller.GetById(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            // Arrange
            _mockMovieFeatures
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Movie)null);

            // Act
            var result = await _controller.GetById(999); // Non-existent ID

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsMovie_WhenMovieExists()
        {
            // Arrange
            var expectedMovie = new Movie 
            { 
                Id = 1, 
                Title = "Test Movie", 
                Description = "Test Description",
                IsAvailable = true, 
                Stock = 5,
                RentalPrice = 4.99m,
                SalePrice = 19.99m,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow
            };

            _mockMovieFeatures
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedMovie);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Movie>(okResult.Value);
            
            Assert.Equal(expectedMovie.Id, returnValue.Id);
            Assert.Equal(expectedMovie.Title, returnValue.Title);
            Assert.Equal(expectedMovie.Description, returnValue.Description);
            Assert.Equal(expectedMovie.IsAvailable, returnValue.IsAvailable);
            Assert.Equal(expectedMovie.Stock, returnValue.Stock);
            Assert.Equal(expectedMovie.RentalPrice, returnValue.RentalPrice);
            Assert.Equal(expectedMovie.SalePrice, returnValue.SalePrice);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockMovieFeatures
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving the movie.", statusCodeResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenMovieIsNull()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");
            var movie = new Movie { Id = 1, Title = "" }; // Invalid movie with empty title

            // Act
            var result = await _controller.Create(movie);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenMovieIsValid()
        {
            // Arrange
            var newMovie = new Movie 
            { 
                Title = "New Movie", 
                Description = "New Description",
                IsAvailable = true, 
                Stock = 10,
                RentalPrice = 3.99m,
                SalePrice = 14.99m
            };

            var createdMovie = new Movie 
            { 
                Id = 1, 
                Title = newMovie.Title,
                Description = newMovie.Description,
                IsAvailable = newMovie.IsAvailable,
                Stock = newMovie.Stock,
                RentalPrice = newMovie.RentalPrice,
                SalePrice = newMovie.SalePrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockMovieFeatures
                .Setup(x => x.SaveAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdMovie);

            // Act
            var result = await _controller.Create(newMovie);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Movie>(createdAtActionResult.Value);
            
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(MovieController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(createdMovie.Id, returnValue.Id);
            Assert.Equal(createdMovie.Title, returnValue.Title);
            Assert.Equal(createdMovie.Description, returnValue.Description);
            Assert.Equal(createdMovie.IsAvailable, returnValue.IsAvailable);
            Assert.Equal(createdMovie.Stock, returnValue.Stock);
            Assert.Equal(createdMovie.RentalPrice, returnValue.RentalPrice);
            Assert.Equal(createdMovie.SalePrice, returnValue.SalePrice);
            
            // Verify the route values
            Assert.NotNull(createdAtActionResult.RouteValues);
            Assert.Equal(createdMovie.Id, createdAtActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Create_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var movie = new Movie { Title = "New Movie" };
            
            _mockMovieFeatures
                .Setup(x => x.SaveAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Create(movie);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while creating the movie.", statusCodeResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenModelIsValid()
        {
            // Arrange
            var movie = new Movie 
            { 
                Title = "New Movie", 
                Description = "Description",
                IsAvailable = true,
                Stock = 5,
                RentalPrice = 5.99m,
                SalePrice = 19.99m
            };

            _mockMovieFeatures
                .Setup(x => x.SaveAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Movie m, CancellationToken _) => 
                {
                    m.Id = 1;
                    return m;
                });

            // Act
            var result = await _controller.Create(movie);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Movie>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("New Movie", returnValue.Title);
        }
    }
}
