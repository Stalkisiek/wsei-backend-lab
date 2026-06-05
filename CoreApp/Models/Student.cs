namespace CoreApp.Models;

public class Student : Person
{
    public string StudentId { get; set; } = string.Empty;
    public int YearOfStudy { get; set; }
    public string EnrollmentYear { get; set; } = string.Empty;
    public AcademicYear? AcademicYear {get;set;}
    public DegreeProgram? DegreeProgram {get;set;}
    public StudentStatus Status { get; set; }
    public List<Grade>  Grades { get; set; }
    public string ProgramName { get; set; } = string.Empty;
}