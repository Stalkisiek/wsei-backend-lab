namespace CoreApp.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Email { get; set; }
    public List<StudyYearField> StudyYearFields { get; set; }
}