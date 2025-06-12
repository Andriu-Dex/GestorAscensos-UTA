using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class DocumentObservation
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int ReviewerId { get; set; }

    public int DocumentId { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual Teacher Reviewer { get; set; } = null!;
}
