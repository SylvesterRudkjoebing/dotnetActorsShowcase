using ActorsShowcase.Server;
using ActorsShowcase.Server.Services;
using ActorsShowcase.Shared.Dtos;
using ActorsShowcase.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Add this using statement
using Moq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ActorsShowcase.Tests.Services
{
    public class ShortestPathTests : IClassFixture<FilmDbContextFixture>
    {
        private readonly FilmDbContext _context;
        private readonly ShortestPath _shortestPathService;
        private readonly ILogger<ShortestPathTests> _logger; // Add logger

        public ShortestPathTests(FilmDbContextFixture fixture)
        {
            _context = fixture.Context;
            _shortestPathService = new ShortestPath(_context);
            // Set up logger
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ShortestPathTests>();
        }

        [Fact]
        public async Task LoadDataAsync_PopulatesDataCorrectly()
        {


            // Act
            await _shortestPathService.LoadDataAsync();

            // Assert
            var people = _shortestPathService.GetPeople();
            var movies = _shortestPathService.GetMovies();

            _logger.LogInformation("People in the database:");
            foreach (var person in people)
            {
                _logger.LogInformation($"ID: {person.Value.Id}, Name: {person.Value.Name}, Birth Year: {person.Value.Birth}");
            }

            _logger.LogInformation("Movies in the database:");
            foreach (var movie in movies)
            {
                _logger.LogInformation($"ID: {movie.Value.Id}, Title: {movie.Value.Title}, Year: {movie.Value.Year}");
            }

            Assert.NotNull(people);
            Assert.NotNull(movies);

            Assert.Equal(2, people.Count);  // 2 people from the seed data
            Assert.Equal(2, movies.Count);  // 2 movies from the seed data
        }

        [Fact]
        public async Task PersonIdForName_ReturnsCorrectPersonId_WhenPersonExists()
        {
            // Arrange
            await _shortestPathService.LoadDataAsync();

            // Act
            var personId = await _shortestPathService.PersonIdForName("John Doe");

            // Assert
            Assert.NotNull(personId);
            Assert.Equal(1, personId);  // John Doe has ID 1 in seed data
        }

        [Fact]
        public async Task PersonIdForName_ReturnsNull_WhenPersonDoesNotExist()
        {
            // Arrange
            await _shortestPathService.LoadDataAsync();

            // Act
            var personId = await _shortestPathService.PersonIdForName("Non Existent Person");

            // Assert
            Assert.Null(personId);  // Should return null for non-existent person
        }

        [Fact]
        public async Task FindShortestPathAsync_ReturnsCorrectPath_WhenPathExists()
        {
            // Arrange
            await _shortestPathService.LoadDataAsync(); // Load data before testing
            var sourceId = 1; // John Doe
            var targetId = 2; // Jane Doe

            // Act
            var path = await _shortestPathService.FindShortestPath(sourceId, targetId); // Await the async method

            // Assert
            Assert.NotNull(path);
            Assert.Single(path); // Only one movie connects them
            Assert.Equal(1, path[0].MovieId); // They are both in "Inception" (Movie 1)
        }

        [Fact]
        public async Task FindShortestPathAsync_ReturnsNull_WhenNoPathExists()
        {
            // Arrange
            await _shortestPathService.LoadDataAsync();
            var sourceId = 1; // John Doe
            var targetId = 999; // Non-existent person ID

            // Act
            var path = await _shortestPathService.FindShortestPath(sourceId, targetId);

            // Assert
            Assert.Null(path); // No path should exist between John Doe and a non-existent person
        }

        [Fact]
        public void NeighborsForPerson_ReturnsCorrectNeighbors()
        {
            // Arrange
            _shortestPathService.LoadDataAsync().Wait();
            var personId = 1; // John Doe has starred in two movies: "Inception" and "The Matrix"

            // Act
            var neighbors = _shortestPathService.NeighborsForPerson(personId);

            // Assert
            Assert.NotNull(neighbors);
            Assert.Contains((1, 2), neighbors); // Jane Doe in "Inception"

            // Øv: Genlæs ShortestPath, hvad den returnerer, hvordan ting gemmes i DTO. .NET Blazor WASM Project --> SPA med mest muligt på client side.
            // bliv bedre til debugging / det røde stopmærkater
            // .NET: Websocket? SignalR?
        }
    }
}