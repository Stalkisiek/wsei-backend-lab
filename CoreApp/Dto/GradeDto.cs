using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record GradeDto
{
    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public double GradeValue { get; init; }
    public GradeType GradeType { get; init; }
    public Guid? LecturerId { get; init; }
    public Guid? AcademicYearId { get; init; }
    public DateTime Date { get; init; } = DateTime.UtcNow;
}

