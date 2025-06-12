using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class Document
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string DocumentType { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime? UploadDate { get; set; }

    public bool IsEditable { get; set; }

    public string Department { get; set; } = null!;

    public string IssuingInstitution { get; set; } = null!;

    public int? DurationHours { get; set; }

    public byte[] FileContent { get; set; } = null!;

    public int TeacherId { get; set; }

    public int? ReviewerId { get; set; }

    public int? RequirementId { get; set; }

    public virtual ICollection<DocumentObservation> DocumentObservations { get; set; } = new List<DocumentObservation>();

    public virtual ICollection<PromotionRequest> PromotionRequests { get; set; } = new List<PromotionRequest>();

    public virtual Requirement? Requirement { get; set; }

    public virtual Teacher? Reviewer { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
