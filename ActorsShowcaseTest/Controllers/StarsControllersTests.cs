using ActorsShowcase.Server.Controllers;
using ActorsShowcase.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ActorsShowcase.Tests.Controllers
{
    public class StarsControllerTests : IClassFixture<FilmDbContextFixture>
    {
        private readonly StarsController _controller;
        private readonly FilmDbContextFixture _fixture;

        public StarsControllerTests(FilmDbContextFixture fixture)
        {
            _fixture = fixture;
            _controller = new StarsController(_fixture.Context);
        }

        [Fact]
        public async Task GetStars_ReturnsListOfStars()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            // Act
            var result = await _controller.GetStars();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Star>>>(result);
            var returnValue = Assert.IsType<List<Star>>(actionResult.Value);
            Assert.Equal(3, returnValue.Count); // 3 stars in seeded data
        }

        [Fact]
        public async Task GetStars_ReturnsEmptyList_WhenNoStarsExist()
        {

            // Arrange
            await _fixture.ResetDatabaseAsync();
            _fixture.Context.Stars.RemoveRange(_fixture.Context.Stars); // Clear all stars
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _controller.GetStars();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Star>>>(result);
            var returnValue = Assert.IsType<List<Star>>(actionResult.Value);
            Assert.Empty(returnValue); // Should return an empty list
        }
    }
}
