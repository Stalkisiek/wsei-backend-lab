namespace CoreApp.Models;

public class Student : Person
{
    public Guid StudentId { get; set; }
    public int YearOfStudy { get; set; }
    public AcademicYear? AcademicYear {get;set;}
    public DegreeProgram? DegreeProgram {get;set;}
    public StudentStatus Status { get; set; }
    public List<Grade>  Grades { get; set; }
    public string ProgramName { get; set; } = string.Empty;
}