using System;
using System.Collections.Generic;

namespace ActorsShowcase.Shared.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public short Year { get; set; }
}
