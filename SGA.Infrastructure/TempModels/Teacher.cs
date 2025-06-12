using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class Teacher
{
    public int Id { get; set; }

    public string IdentificationNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int CurrentRank { get; set; }

    public DateTime StartDateInCurrentRank { get; set; }

    public int DaysInCurrentRank { get; set; }

    public int UserTypeId { get; set; }

    public int Works { get; set; }

    public decimal EvaluationScore { get; set; }

    public int TrainingHours { get; set; }

    public int ResearchMonths { get; set; }

    public int YearsInCurrentRank { get; set; }

    public virtual ICollection<AcademicDegree> AcademicDegrees { get; set; } = new List<AcademicDegree>();

    public virtual ICollection<DocumentObservation> DocumentObservations { get; set; } = new List<DocumentObservation>();

    public virtual ICollection<Document> DocumentReviewers { get; set; } = new List<Document>();

    public virtual ICollection<Document> DocumentTeachers { get; set; } = new List<Document>();

    public virtual ICollection<PromotionRequest> PromotionRequestReviewers { get; set; } = new List<PromotionRequest>();

    public virtual ICollection<PromotionRequest> PromotionRequestTeachers { get; set; } = new List<PromotionRequest>();

    public virtual UserType UserType { get; set; } = null!;
}
