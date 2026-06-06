using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreApp.Models;
using Infrastucture.EntityFramework.Entities;
using Infrastucture.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastucture.EntityFramework.Context;

public class DatabaseSeeder : IDataSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public int Order => 2;

    public DatabaseSeeder(AppDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var cancellationToken = CancellationToken.None;

        await _context.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureSchemaCompatibilityAsync(cancellationToken);
        await SeedAuthUsersAsync(cancellationToken);

        var academicYear = await _context.AcademicYears.FirstOrDefaultAsync(cancellationToken);
        if (academicYear == null)
        {
            academicYear = new AcademicYear
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "2025/2026",
                YearFrom = 2025,
                YearTo = 2026,
                IsActive = true
            };
            _context.AcademicYears.Add(academicYear);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var degreeProgram = await _context.DegreePrograms.FirstOrDefaultAsync(cancellationToken);
        if (degreeProgram == null)
        {
            degreeProgram = new DegreeProgram
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Code = "INF-BSC",
                Name = "Informatyka",
                Faculty = "Wydział Informatyki",
                DegreeType = DegreeType.Engineering,
                DurationYears = 3,
                MinEctsForDiploma = 180,
                Courses = new List<Course>()
            };
            _context.DegreePrograms.Add(degreeProgram);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var janLecturerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var annaLecturerId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        Lecturer? lecturer1 = null;
        Lecturer? lecturer2 = null;
        if (!await _context.Lecturers.AnyAsync(cancellationToken))
        {
             lecturer1 = new Lecturer
             {
                 Id = janLecturerId,
                 FirstName = "Jan",
                 LastName = "Kowal",
                 Pesel = null,
                 Email = EmailAddress.From("jan.kowal@wsei.edu.pl"),
                 Title = "Dr",
                 Faculty = "Wydział Informatyki",
                 TaughtCorses = new List<Course>()
             };

             _context.Lecturers.AddRange(lecturer1, new Lecturer
             {
                 Id = annaLecturerId,
                 FirstName = "Anna",
                 LastName = "Nowak",
                 Pesel = null,
                 Email = EmailAddress.From("anna.nowak@wsei.edu.pl"),
                 Title = "Prof",
                 Faculty = "Wydział Matematyki",
                 TaughtCorses = new List<Course>()
             });
            await _context.SaveChangesAsync(cancellationToken);
        }

        lecturer1 ??= await _context.Lecturers.FirstOrDefaultAsync(l => l.Id == janLecturerId, cancellationToken);
        lecturer2 ??= await _context.Lecturers.FirstOrDefaultAsync(l => l.Id == annaLecturerId, cancellationToken);

        Course? course1 = null;
        Course? course2 = null;
        if (!await _context.Courses.AnyAsync(cancellationToken))
        {
            course1 = new Course
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Code = "CS101",
                Name = "Wprowadzenie do informatyki",
                EctsCredits = 5,
                CompletionType = CompletionType.Exam,
                AcademicYear = academicYear,
                DegreeProgram = degreeProgram,
                Enrollments = new List<Student>()
            };

            course2 = new Course
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Code = "MATH100",
                Name = "Analiza matematyczna",
                EctsCredits = 6,
                CompletionType = CompletionType.Exam,
                AcademicYear = academicYear,
                DegreeProgram = degreeProgram,
                Enrollments = new List<Student>()
            };

            _context.Courses.AddRange(course1, course2);
            await _context.SaveChangesAsync(cancellationToken);
        }

        course1 ??= await _context.Courses.Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Code == "CS101", cancellationToken);
        course2 ??= await _context.Courses.Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Code == "MATH100", cancellationToken);

        Student? student1 = null;
        Student? student2 = null;
        if (!await _context.Students.AnyAsync(cancellationToken))
        {
             student1 = new Student
             {
                 Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                 StudentId = "ALB-2025-0001",
                 FirstName = "Piotr",
                 LastName = "Zieliński",
                 Pesel = null,
                 Email = EmailAddress.From("piotr.zielinski@wsei.edu.pl"),
                 YearOfStudy = 1,
                 EnrollmentYear = "2025/2026",
                 AcademicYear = academicYear,
                 DegreeProgram = degreeProgram,
                 Status = StudentStatus.Active,
                 ProgramName = degreeProgram.Code,
                 Grades = new List<Grade>()
             };

             student2 = new Student
             {
                 Id = Guid.Parse("66666666-7777-8888-9999-000000000000"),
                 StudentId = "ALB-2024-0002",
                 FirstName = "Alicja",
                 LastName = "Maj",
                 Pesel = null,
                 Email = EmailAddress.From("alicja.maj@wsei.edu.pl"),
                 YearOfStudy = 2,
                 EnrollmentYear = "2024/2025",
                 AcademicYear = academicYear,
                 DegreeProgram = degreeProgram,
                 Status = StudentStatus.Active,
                 ProgramName = degreeProgram.Code,
                 Grades = new List<Grade>()
             };

             _context.Students.AddRange(student1, student2);
            await _context.SaveChangesAsync(cancellationToken);
        }

        student1 ??= await _context.Students.FirstOrDefaultAsync(s => s.StudentId == "ALB-2025-0001", cancellationToken);
        student2 ??= await _context.Students.FirstOrDefaultAsync(s => s.StudentId == "ALB-2024-0002", cancellationToken);

        var changedCourses = false;
        if (course1 != null && lecturer1 != null && course1.Lecturer?.Id != lecturer1.Id)
        {
            course1.Lecturer = lecturer1;
            changedCourses = true;
        }

        if (course2 != null && lecturer1 != null && course2.Lecturer?.Id != lecturer1.Id)
        {
            course2.Lecturer = lecturer1;
            changedCourses = true;
        }

        if (changedCourses)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (course1 != null && student1 != null)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Students SET CourseId = {course1.Id} WHERE Id = {student1.Id}",
                cancellationToken);
        }

        if (course2 != null && student2 != null)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE Students SET CourseId = {course2.Id} WHERE Id = {student2.Id}",
                cancellationToken);
        }

        var grade1Id = Guid.Parse("99999999-8888-7777-6666-555555555555");
        var grade2Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        if (student1 != null && course1 != null && lecturer1 != null &&
            !await _context.Grades.AnyAsync(g => g.Id == grade1Id, cancellationToken))
        {
            _context.Grades.Add(new Grade
            {
                Id = grade1Id,
                Student = student1,
                Course = course1,
                Lecturer = lecturer1,
                AcademicYear = academicYear,
                Date = DateTime.UtcNow.Date,
                GradeType = GradeType.Final,
                GradeValue = GradeValue.Grade45
            });
        }

        if (student2 != null && course2 != null && lecturer1 != null &&
            !await _context.Grades.AnyAsync(g => g.Id == grade2Id, cancellationToken))
        {
            _context.Grades.Add(new Grade
            {
                Id = grade2Id,
                Student = student2,
                Course = course2,
                Lecturer = lecturer1,
                AcademicYear = academicYear,
                Date = DateTime.UtcNow.Date,
                GradeType = GradeType.Partial,
                GradeValue = GradeValue.Grade40
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var students = await _context.Students.OrderBy(s => s.Id).ToListAsync(cancellationToken);
        var usedCodes = new HashSet<string>(
            students
                .Select(s => (s.StudentId ?? string.Empty).Trim())
                .Where(code => !string.IsNullOrWhiteSpace(code) && !Guid.TryParse(code, out _)),
            StringComparer.OrdinalIgnoreCase);

        var nextLegacyNumber = 1;
        var changedAny = false;
        foreach (var student in students)
        {
            var current = (student.StudentId ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(current) && !Guid.TryParse(current, out _))
            {
                continue;
            }

            string candidate;
            do
            {
                candidate = $"ALB-LEGACY-{nextLegacyNumber:0000}";
                nextLegacyNumber++;
            } while (usedCodes.Contains(candidate));

            student.StudentId = candidate;
            usedCodes.Add(candidate);
            changedAny = true;
        }

        if (changedAny)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Domain database seeding finished.");
    }

    private async Task SeedAuthUsersAsync(CancellationToken cancellationToken)
    {
        const string adminRoleId = "11111111-1111-1111-1111-111111111111";
        const string lecturerRoleId = "22222222-2222-2222-2222-111111111111";
        const string deanOfficeRoleId = "33333333-3333-3333-3333-111111111111";

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == adminRoleId, cancellationToken);
        if (!roleExists)
        {
            _logger.LogWarning("Admin role is missing - skipping auth users seeding.");
            return;
        }

        var passwordHasher = new PasswordHasher<AppUser>();

        var seedUsers = new[]
        {
            new
            {
                User = new AppUser
                {
                    Id = "22222222-2222-2222-2222-222222222222",
                    UserName = "admin1@example.local",
                    NormalizedUserName = "ADMIN1@EXAMPLE.LOCAL",
                    Email = "admin1@example.local",
                    NormalizedEmail = "ADMIN1@EXAMPLE.LOCAL",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "One",
                    FullName = "Admin One",
                    Department = "IT",
                    Status = SystemUserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                },
                Password = "Admin@123!",
                RoleIds = new[] { adminRoleId }
            },
            new
            {
                User = new AppUser
                {
                    Id = "33333333-3333-3333-3333-333333333333",
                    UserName = "admin2@example.local",
                    NormalizedUserName = "ADMIN2@EXAMPLE.LOCAL",
                    Email = "admin2@example.local",
                    NormalizedEmail = "ADMIN2@EXAMPLE.LOCAL",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Two",
                    FullName = "Admin Two",
                    Department = "IT",
                    Status = SystemUserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                },
                Password = "Admin@123!",
                RoleIds = new[] { adminRoleId }
            },
            new
            {
                User = new AppUser
                {
                    Id = "44444444-4444-4444-4444-444444444444",
                    UserName = "lecturer1@example.local",
                    NormalizedUserName = "LECTURER1@EXAMPLE.LOCAL",
                    Email = "lecturer1@example.local",
                    NormalizedEmail = "LECTURER1@EXAMPLE.LOCAL",
                    EmailConfirmed = true,
                    FirstName = "Jan",
                    LastName = "Kowal",
                    FullName = "Jan Kowal",
                    Department = "IT",
                    Status = SystemUserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                },
                Password = "Lecturer@123!",
                RoleIds = new[] { lecturerRoleId }
            },
            new
            {
                User = new AppUser
                {
                    Id = "55555555-5555-5555-5555-555555555555",
                    UserName = "deanoffice1@example.local",
                    NormalizedUserName = "DEANOFFICE1@EXAMPLE.LOCAL",
                    Email = "deanoffice1@example.local",
                    NormalizedEmail = "DEANOFFICE1@EXAMPLE.LOCAL",
                    EmailConfirmed = true,
                    FirstName = "Magda",
                    LastName = "Dziekan",
                    FullName = "Magda Dziekan",
                    Department = "Dean Office",
                    Status = SystemUserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                },
                Password = "DeanOffice@123!",
                RoleIds = new[] { deanOfficeRoleId }
            }
        };

        foreach (var seed in seedUsers)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == seed.User.Id, cancellationToken);
            if (existing == null)
            {
                seed.User.PasswordHash = passwordHasher.HashPassword(seed.User, seed.Password);
                seed.User.SecurityStamp = Guid.NewGuid().ToString();
                seed.User.ConcurrencyStamp = Guid.NewGuid().ToString();
                _context.Users.Add(seed.User);
                existing = seed.User;
            }
            else if (string.IsNullOrWhiteSpace(existing.PasswordHash))
            {
                existing.PasswordHash = passwordHasher.HashPassword(existing, seed.Password);
                existing.SecurityStamp ??= Guid.NewGuid().ToString();
                existing.ConcurrencyStamp = Guid.NewGuid().ToString();
                _context.Users.Update(existing);
            }

            existing.UserName = seed.User.UserName;
            existing.NormalizedUserName = seed.User.NormalizedUserName;
            existing.Email = seed.User.Email;
            existing.NormalizedEmail = seed.User.NormalizedEmail;
            existing.FirstName = seed.User.FirstName;
            existing.LastName = seed.User.LastName;
            existing.FullName = seed.User.FullName;
            existing.Department = seed.User.Department;
            existing.Status = seed.User.Status;
            existing.EmailConfirmed = seed.User.EmailConfirmed;
            _context.Users.Update(existing);

            foreach (var roleId in seed.RoleIds)
            {
                var roleAssigned = await _context.Set<IdentityUserRole<string>>()
                    .AnyAsync(ur => ur.UserId == existing.Id && ur.RoleId == roleId, cancellationToken);
                if (!roleAssigned)
                {
                    _context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
                    {
                        UserId = existing.Id,
                        RoleId = roleId
                    });
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSchemaCompatibilityAsync(CancellationToken cancellationToken)
    {
        if (!_context.Database.IsSqlite())
            return;

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info('Courses')";

        var hasSemester = false;
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var columnName = reader["name"]?.ToString();
                if (string.Equals(columnName, "Semester", StringComparison.OrdinalIgnoreCase))
                {
                    hasSemester = true;
                    break;
                }
            }
        }

        if (!hasSemester)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE Courses ADD COLUMN Semester INTEGER NOT NULL DEFAULT 0",
                cancellationToken);
        }
    }
}

