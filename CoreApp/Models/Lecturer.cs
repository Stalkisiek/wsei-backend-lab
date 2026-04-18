namespace CoreApp.Models;

public class Lecturer : Person
{
    public string Title { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public List<Course> TaughtCorses { get; set; } = new List<Course>();
}