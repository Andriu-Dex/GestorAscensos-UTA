using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class PromotionRequest
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int CurrentRank { get; set; }

    public int TargetRank { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? Comments { get; set; }

    public int? DocumentId { get; set; }

    public int? ReviewerId { get; set; }

    public virtual Document? Document { get; set; }

    public virtual ICollection<PromotionObservation> PromotionObservations { get; set; } = new List<PromotionObservation>();

    public virtual Teacher? Reviewer { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
