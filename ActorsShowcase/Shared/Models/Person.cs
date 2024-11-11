using System;
using System.Collections.Generic;

namespace ActorsShowcase.Shared.Models;

public partial class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Birth { get; set; }
}
