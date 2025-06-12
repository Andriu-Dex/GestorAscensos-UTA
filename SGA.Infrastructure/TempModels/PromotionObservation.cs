using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class PromotionObservation
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int PromotionRequestId { get; set; }

    public virtual PromotionRequest PromotionRequest { get; set; } = null!;
}
