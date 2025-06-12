using System;
using System.Collections.Generic;

namespace SGA.Infrastructure.TempModels;

public partial class AcademicDegree
{
    public int Id { get; set; }

    public string DegreeType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string IssuingInstitution { get; set; } = null!;

    public int TeacherId { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
