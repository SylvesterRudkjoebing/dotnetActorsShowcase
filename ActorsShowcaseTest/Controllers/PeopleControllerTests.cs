using ActorsShowcase.Server.Controllers;
using ActorsShowcase.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ActorsShowcase.Tests.Controllers
{
    public class PeopleControllerTests : IClassFixture<FilmDbContextFixture>
    {
        private readonly PeopleController _controller;
        private readonly FilmDbContextFixture _fixture;

        public PeopleControllerTests(FilmDbContextFixture fixture)
        {
            _fixture = fixture;
            _controller = new PeopleController(_fixture.Context); // Injecting the InMemory context
        }

        [Fact]
        public async Task GetPeople_ReturnsListOfPeople()
        {
            // Arrange
            await _fixture.ResetDatabaseAsync();

            // Act
            var result = await _controller.GetPeople();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Person>>>(result);
            var returnValue = Assert.IsType<List<Person>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count); // 2 people in seeded data
        }

        [Fact]
        public async Task GetPeople_ReturnsEmptyList_WhenNoPeopleExist()
        {

            // Arrange
            await _fixture.ResetDatabaseAsync();
            _fixture.Context.People.RemoveRange(_fixture.Context.People); // Clear all people
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPeople();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Person>>>(result);
            var returnValue = Assert.IsType<List<Person>>(actionResult.Value);
            Assert.Empty(returnValue); // Should return an empty list
        }


        // Additional tests (e.g., searching by name, filtering by birth year) could go here...
    }
}
