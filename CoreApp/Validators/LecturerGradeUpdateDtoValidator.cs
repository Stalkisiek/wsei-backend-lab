using FluentValidation;
using CoreApp.Dto;

namespace CoreApp.Validators;

public class LecturerGradeUpdateDtoValidator : AbstractValidator<LecturerGradeUpdateDto>
{
    public LecturerGradeUpdateDtoValidator()
    {
        RuleFor(x => x.GradeValue)
            .GreaterThanOrEqualTo(1.0).WithMessage("Ocena musi być >= 1.0")
            .LessThanOrEqualTo(5.0).WithMessage("Ocena musi być <= 5.0");

        RuleFor(x => x.GradeType)
            .NotEmpty().WithMessage("Typ oceny jest wymagany.")
            .Must(x => x == "Exam" || x == "Midterm" || x == "Assignment" || x == "Attendance")
            .WithMessage("Typ oceny musi być jednym z: Exam, Midterm, Assignment, Attendance");

        RuleFor(x => x.Date)
            .Must(d => d == null || d <= DateTime.UtcNow)
            .WithMessage("Data oceny nie może być w przyszłości.");
    }
}

