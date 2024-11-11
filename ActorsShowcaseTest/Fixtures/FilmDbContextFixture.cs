using ActorsShowcase.Server;
using ActorsShowcase.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FilmDbContextFixture : IDisposable
{
    public FilmDbContext Context { get; private set; }

    public FilmDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<FilmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique in-memory database name
            .Options;

        Context = new FilmDbContext(options);
        SeedData(); // Initial seed for tests
    }

    private void SeedData()
    {
        // Clear any existing data and add initial entities
        Context.People.RemoveRange(Context.People);
        Context.Movies.RemoveRange(Context.Movies);
        Context.Stars.RemoveRange(Context.Stars);

        Console.WriteLine("Starting database seeding...");

        // Seed people
        Context.People.AddRange(new List<Person>
        {
            new Person { Id = 1, Name = "John Doe", Birth = 1990 },
            new Person { Id = 2, Name = "Jane Doe", Birth = 1992 }
        });

        // Seed movies
        Context.Movies.AddRange(new List<Movie>
        {
            new Movie { Id = 1, Title = "Inception", Year = 2010 },
            new Movie { Id = 2, Title = "The Matrix", Year = 1999 }
        });

        // Seed stars
        Context.Stars.AddRange(new List<Star>
        {
            new Star { id = 1, MovieId = 1, PersonId = 1 },
            new Star { id = 2, MovieId = 1, PersonId = 2 },
            new Star { id = 3, MovieId = 2, PersonId = 1 }
        });

        Context.SaveChanges();

        Console.WriteLine("Database seeded successfully.");
    }

    public void VerifyDatabaseSeeded()
    {
        // Check if 'People' table is seeded with the expected records
        if (Context.People.Count() != 2)
            throw new Exception("Expected 2 people, but found a different count.");

        if (Context.People.FirstOrDefault(p => p.Name == "John Doe" && p.Birth == 1990) == null)
            throw new Exception("Expected person 'John Doe' with Birth 1990 not found.");

        if (Context.People.FirstOrDefault(p => p.Name == "Jane Doe" && p.Birth == 1992) == null)
            throw new Exception("Expected person 'Jane Doe' with Birth 1992 not found.");

        // Verify movies table
        if (Context.Movies.Count() != 2)
            throw new Exception("Expected 2 movies, but found a different count.");

        if (Context.Movies.FirstOrDefault(m => m.Title == "Inception" && m.Year == 2010) == null)
            throw new Exception("Expected movie 'Inception' with Year 2010 not found.");

        if (Context.Movies.FirstOrDefault(m => m.Title == "The Matrix" && m.Year == 1999) == null)
            throw new Exception("Expected movie 'The Matrix' with Year 1999 not found.");

        // Verify stars table
        if (Context.Stars.Count() != 3)
            throw new Exception("Expected 3 stars, but found a different count.");

        Console.WriteLine("Database verification completed successfully.");
    }

    public async Task ResetDatabaseAsync()
    {
        Context.People.RemoveRange(Context.People);
        Context.Movies.RemoveRange(Context.Movies);
        Context.Stars.RemoveRange(Context.Stars);
        await Context.SaveChangesAsync();
        SeedData(); // Reseed the database after clearing
        VerifyDatabaseSeeded();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted(); // Cleanup in-memory database
        Context.Dispose();
    }
}
