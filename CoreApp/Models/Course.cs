namespace CoreApp.Models;

public class Course : EntityBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int EctsCredits { get; set; }
    public CompletionType CompletionType { get; set; }

    public AcademicYear? AcademicYear { get; set; }
    public DegreeProgram? DegreeProgram { get; set; }
    public List<Student> Enrollments { get; set; }
    
}