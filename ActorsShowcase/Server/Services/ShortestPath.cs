using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ActorsShowcase.Shared.Dtos;
using Microsoft.Extensions.Logging; // Add this namespace
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;


namespace ActorsShowcase.Server.Services
{
    public class ShortestPath : IShortestPath
    {
        private readonly FilmDbContext _context;
        private bool _dataLoaded = false;

        public ShortestPath(FilmDbContext context)
        {
            _context = context;
        }

        // Maps names to a set of corresponding person_ids
        private static Dictionary<string, HashSet<int>> names = new Dictionary<string, HashSet<int>>();

        // Maps person_ids to a dictionary of PersonDto
        private static Dictionary<int, PersonDto> people = new Dictionary<int, PersonDto>();

        // Maps movie_ids to a dictionary of MovieDto
        private static Dictionary<int, MovieDto> movies = new Dictionary<int, MovieDto>();

        public Dictionary<int, MovieDto> GetMovies()
        {
            return movies;
        }

        public Dictionary<int, PersonDto> GetPeople()
        {
            return people;
        }

        public class Node
        {
            public int State { get; }
            public Node Parent { get; }
            public int Action { get; }

            public Node(int state, Node parent, int action)
            {
                State = state;
                Parent = parent;
                Action = action;
            }
        }


        class QueueFrontier
        {
            public List<Node> Frontier = new List<Node>();

            public void Add(Node node) => Frontier.Add(node);

            public bool ContainsState(int state) => Frontier.Any(n => n.State == state);

            public bool Empty() => Frontier.Count == 0;

            public Node Remove()
            {
                if (Empty()) throw new Exception("empty frontier");
                var node = Frontier[0];
                Frontier.RemoveAt(0);
                return node;
            }
        }


        public async Task LoadDataAsync()
        {
            if (_dataLoaded) return;

            // Load people
            var peopleList = await _context.People
                .Select(person => new PersonDto
                {
                    Id = person.Id,
                    Name = person.Name,
                    Birth = person.Birth,
                    Stars = new HashSet<StarDto>()
                })
                .ToListAsync();

            foreach (var person in peopleList)
            {
                people[person.Id] = person;
                string lowerName = person.Name.ToLower();
                if (!names.ContainsKey(lowerName))
                {
                    names[lowerName] = new HashSet<int>();
                }
                names[lowerName].Add(person.Id);
            }

            // Load movies
            var moviesList = await _context.Movies
                .Select(movie => new MovieDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Year = movie.Year,
                    Stars = new HashSet<StarDto>()
                })
                .ToListAsync();

            foreach (var movie in moviesList)
            {
                movies[movie.Id] = movie;
            }

            // Load stars
            var starsList = await _context.Stars
                .Select(star => new StarDto
                {
                    Id = star.id,
                    PersonId = star.PersonId,
                    MovieId = star.MovieId
                })
                .ToListAsync();

            foreach (var star in starsList)
            {
                if (people.ContainsKey(star.PersonId))
                {
                    people[star.PersonId].Stars.Add(star);
                }

                if (movies.ContainsKey(star.MovieId))
                {
                    movies[star.MovieId].Stars.Add(star);
                }
            }

            _dataLoaded = true;
        }

        public async Task<int?> PersonIdForName(string name)
        {
            var lowerName = name.ToLower();  // Convert search term to lowercase
            var personIds = await _context.People
                .Where(person => person.Name.ToLower() == lowerName)
                .Select(person => person.Id)
                .ToListAsync();

            if (personIds.Count == 0)
            {
                return null; // Person not found
            }
            else if (personIds.Count > 1)
            {
                return personIds.First(); // Handle multiple matches differently in web
            }
            else
            {
                return personIds.First(); // Single match found
            }
        }


        public async Task<List<(int MovieId, int PersonId)>> FindShortestPath(int source, int target)
        {
            var start = new Node(source, null, -1); // Dummy action for the start node
            var frontier = new QueueFrontier();
            frontier.Add(start);

            var explored = new HashSet<int>();

            while (true)
            {
                if (frontier.Empty())
                {
                    Console.WriteLine("No path found.");
                    return null; // No path found
                }

                var node = frontier.Remove();
                explored.Add(node.State);

                Console.WriteLine($"Exploring node: {node.State}");

                foreach (var (movieId, personId) in NeighborsForPerson(node.State))
                {
                    Console.WriteLine($"Checking neighbor: {personId} through movie {movieId}");

                    if (personId == target)
                    {
                        var path = new List<(int MovieId, int PersonId)>();
                        var currentNode = new Node(personId, node, movieId); // Final target node

                        while (currentNode.Parent != null)
                        {
                            // Add (movieId, personId)
                            path.Add((currentNode.Action, currentNode.State));
                            currentNode = currentNode.Parent;
                        }

                        // Ensure to reverse the path to get the correct order
                        path.Reverse();

                        return path;
                    }

                    if (!frontier.ContainsState(personId) && !explored.Contains(personId))
                    {
                        Console.WriteLine($"Adding {personId} to the frontier with movie {movieId}.");
                        var newNode = new Node(personId, node, movieId);
                        frontier.Add(newNode);
                    }
                }
            }
        }

        public HashSet<(int, int)> NeighborsForPerson(int personId)
        {
            if (!people.ContainsKey(personId) || people[personId].Stars == null)
            {
                Console.WriteLine($"No stars found for person {personId}.");
                return new HashSet<(int, int)>(); // Return empty set if no stars
            }

            var movieIds = people[personId].Stars.Select(s => s.MovieId).ToHashSet();
            var neighbors = new HashSet<(int, int)>();

            foreach (var movieId in movieIds)
            {
                Console.WriteLine($"Checking movie {movieId} for neighbors.");
                foreach (var star in movies[movieId].Stars)
                {
                    if (star.PersonId != personId)
                    {
                        Console.WriteLine($"Found neighbor: {star.PersonId} in movie {movieId}.");
                        neighbors.Add((movieId, star.PersonId));
                    }
                }
            }

            return neighbors;
        }

    }
}
