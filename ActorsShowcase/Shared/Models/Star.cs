using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActorsShowcase.Shared.Models;

public partial class Star
{
    [Key]
    public int id { get; set; }

    [ForeignKey("Person")]
    public int PersonId { get; set; }

    [ForeignKey("Movie")]
    public int MovieId { get; set; }
}
