using System.Collections.Generic;
using System.Threading.Tasks;
using ActorsShowcase.Shared.Dtos;

namespace ActorsShowcase.Server.Services
{
    public interface IShortestPath
    {
        Task<List<(int MovieId, int PersonId)>> FindShortestPath(int source, int target);
        Task LoadDataAsync();

        Dictionary<int, MovieDto> GetMovies();
        Dictionary<int, PersonDto> GetPeople();
    }
}
