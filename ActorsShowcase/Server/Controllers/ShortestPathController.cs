using ActorsShowcase.Server;
using ActorsShowcase.Server.Services;
using ActorsShowcase.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ShortestPathController : ControllerBase
{
    private readonly IShortestPath _shortestPath;
    private readonly FilmDbContext _context;

    public ShortestPathController(IShortestPath shortestPath, FilmDbContext context)
    {
        _shortestPath = shortestPath;
        _context = context;
    }

    [HttpGet("loaddata")]
    public async Task<IActionResult> LoadData()
    {
        try
        {
            await _shortestPath.LoadDataAsync();
            return Ok("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("findpath")]
    public async Task<IActionResult> FindShortestPath(string sourceName, string targetName)
    {
        try
        {
            var sourceId = await GetPersonIdByNameAsync(sourceName);
            var targetId = await GetPersonIdByNameAsync(targetName);

            if (!sourceId.HasValue || !targetId.HasValue)
            {
                return BadRequest("One or both of the names could not be resolved.");
            }

            var foundPath = await _shortestPath.FindShortestPath(sourceId.Value, targetId.Value);

            if (foundPath == null || foundPath.Count == 0)
            {
                return NotFound("No path found.");
            }

            // Mapping the path to PathDto objects with movie titles and person names to send to the client
            var pathDtos = new List<PathDto>();
            foreach (var step in foundPath)
            {
                var movie = _shortestPath.GetMovies()[step.MovieId]; // Get movie by ID
                var person = _shortestPath.GetPeople()[step.PersonId]; // Get person by ID

                pathDtos.Add(new PathDto
                {
                    MovieId = step.MovieId,
                    PersonId = step.PersonId,
                    MovieTitle = movie.Title,   // Add movie title
                    PersonName = person.Name    // Add person name
                });
            }

            return Ok(pathDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    private async Task<int?> GetPersonIdByNameAsync(string name)
    {
        var lowerName = name.ToLower();  // Convert search term to lowercase

        var person = await _context.People
            .Where(p => p.Name.ToLower() == lowerName)
            .FirstOrDefaultAsync();

        return person?.Id;
    }
}
