using FluentValidation;
using CoreApp.Dto;
using System;

namespace CoreApp.Validators;

public class StudentCreateDtoValidator : AbstractValidator<StudentCreateDto>
{
    public StudentCreateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może przekraczać 100 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Imię zawiera niedozwolone znaki.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(200).WithMessage("Nazwisko nie może przekraczać 200 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwisko zawiera niedozwolone znaki.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany.")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email.")
            .MaximumLength(200).WithMessage("Email nie może przekraczać 200 znaków.");

        RuleFor(x => x.YearOfStudy)
            .Must(year => year >= 1 && year <= 5)
            .WithMessage("Niepoprawny rok studiów.");

        RuleFor(x => x.ProgramCode)
            .NotEmpty().WithMessage("Kod programu jest wymagany.")
            .MaximumLength(200).WithMessage("Kod programu jest za długi.");

        RuleFor(x => x.StudentId)
            .Must(id => string.IsNullOrEmpty(id) || Guid.TryParse(id, out _))
            .WithMessage("StudentId musi być poprawnym identyfikatorem GUID.");

        RuleFor(x => x.EnrollmentYearFrom)
            .GreaterThanOrEqualTo(1900).WithMessage("Rok rozpoczęcia musi być poprawny.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Rok rozpoczęcia nie może być w przyszłości.");
    }
}

