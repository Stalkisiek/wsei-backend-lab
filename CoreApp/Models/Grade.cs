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

    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }

    public List<GradeChangeHistory> ChangeHistory { get; set; } = new List<GradeChangeHistory>();
}

public class GradeChangeHistory : EntityBase
{
    public Guid GradeId { get; set; }
    public Grade? Grade { get; set; }
    public GradeValue? PreviousValue { get; set; }
    public GradeValue NewValue { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}


