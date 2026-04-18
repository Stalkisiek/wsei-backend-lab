namespace CoreApp.Models;

public class Grade : EntityBase
{
    public Student Student { get; set; }
    public Course Course { get; set; }
    public DateTime Date { get; set; }
    public GradeType GradeType { get; set; }
    public Lecturer? Lecturer { get; set; }
    public AcademicYear? AcademicYear { get; set; }
    public GradeValue GradeValue { get; set; }
}