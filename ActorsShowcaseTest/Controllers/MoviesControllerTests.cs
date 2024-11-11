using ActorsShowcase.Server.Controllers;
using ActorsShowcase.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ActorsShowcase.Tests.Controllers
{
    public class MoviesControllerTests : IClassFixture<FilmDbContextFixture>
    {
        private readonly MoviesController _controller;
        private readonly FilmDbContextFixture _fixture;

        public MoviesControllerTests(FilmDbContextFixture fixture)
        {
            _fixture = fixture;
            _controller = new MoviesController(_fixture.Context);
        }

        [Fact]
        public async Task GetMovies_ReturnsListOfMovies()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            // Act
            var result = await _controller.GetMovies();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Movie>>>(result);
            var returnValue = Assert.IsType<List<Movie>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count); // 2 movies in seeded data
        }

        [Fact]
        public async Task GetMovies_ReturnsEmptyList_WhenNoMoviesExist()
        {

            // Arrange
            await _fixture.ResetDatabaseAsync();
            _fixture.Context.Movies.RemoveRange(_fixture.Context.Movies); // Clear all movies
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _controller.GetMovies();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Movie>>>(result);
            var returnValue = Assert.IsType<List<Movie>>(actionResult.Value);
            Assert.Empty(returnValue); // Should return an empty list
        }
    }
}
