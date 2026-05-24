using System;
using CoreApp.Models;

namespace CoreApp.Dto;

public sealed record GradeUpdateDto
{
    public double GradeValue { get; init; }
    public GradeType GradeType { get; init; }
    public DateTime Date { get; init; } = DateTime.UtcNow;
}


