namespace CoreApp.Models;

public class DegreeProgram : EntityBase
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public DegreeType  DegreeType { get; set; }
    public int DurationYears { get; set; }
    public int MinEctsForDiploma { get; set; }
    public List<Course> Courses { get; set; }
}