namespace SGA.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para AcademicDegree
    /// </summary>
    public class AcademicDegreeDto
    {
        public int Id { get; set; }
        public string DegreeType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IssuingInstitution { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherFullName { get; set; } = string.Empty;
    }
}
