using System;
namespace ActorsShowcase.Shared.Dtos
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public short Year { get; set; }

        public ICollection<StarDto> Stars { get; set; } = new HashSet<StarDto>();
    }

    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Birth { get; set; }

        public ICollection<StarDto> Stars { get; set; } = new HashSet<StarDto>();
    }

    public class StarDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int MovieId { get; set; }
    }

    public class PathDto
    {
        public int MovieId { get; set; }
        public int PersonId { get; set; }
        public string? MovieTitle { get; set; }  // New property to show ShortestPath chain of connections in razor view
        public string? PersonName { get; set; }  // New property to show ShortestPath chain of connections in razor view
    }

}

