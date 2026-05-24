using FluentValidation;
using CoreApp.Dto;
using System;
using System.Linq;

namespace CoreApp.Validators;

public class GradeDtoValidator : AbstractValidator<GradeDto>
{
    private static readonly double[] Allowed = new[] { 2.0, 3.0, 3.5, 4.0, 4.5, 5.0 };

    public GradeDtoValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("CourseId is required.");
        RuleFor(x => x.LecturerId).Must(id => id == null || id != Guid.Empty).WithMessage("LecturerId must be a valid guid or null.");
        RuleFor(x => x.AcademicYearId).Must(id => id == null || id != Guid.Empty).WithMessage("AcademicYearId must be a valid guid or null.");
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1))
            .WithMessage("Date cannot be in the future.");
        RuleFor(x => x.GradeValue)
            .Must(v => Allowed.Any(a => Math.Abs(a - v) < 0.001))
            .WithMessage($"GradeValue must be one of: {string.Join(", ", Allowed.Select(a => a.ToString("N1")))}.");
    }
}

