using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class Requirement
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int YearsInCurrentRank { get; set; }

    public int RequiredWorks { get; set; }

    public decimal MinimumEvaluationScore { get; set; }

    public int RequiredTrainingHours { get; set; }

    public int RequiredResearchMonths { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
