namespace CoreApp.Models;

public class StudentGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public StudyYear StudyYear { get; set; }
    public List<Student> Students { get; set; }
}