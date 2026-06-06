using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryStudentRepository : MemoryGenericRepository<Student>, IStudentRepository
{
    public MemoryStudentRepository() : base()
    {
        var id1 = Guid.NewGuid();
        var s1 = new Student
        {
            Id = id1,
            StudentId = "ALB-2024-0001",
            FirstName = "Adam",
            LastName = "Nowak",
            Email = EmailAddress.From("adam.nowak@example.com"),
            ProgramName = "Informatyka",
            YearOfStudy = 2,
            EnrollmentYear = "2022",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        };
        _data.Add(s1.Id, s1);

        var id2 = Guid.NewGuid();
        var s2 = new Student
        {
            Id = id2,
            StudentId = "ALB-2024-0002",
            FirstName = "Ewa",
            LastName = "Kowalska",
            Email = EmailAddress.From("ewa.kowalska@example.com"),
            ProgramName = "Matematyka",
            YearOfStudy = 1,
            EnrollmentYear = "2023",
            Status = StudentStatus.Active,
            Grades = new List<Grade>()
        };
        _data.Add(s2.Id, s2);
    }

    public Task<IEnumerable<Student>> FindByAcademicYearAsync(Guid academicYearId)
    {
        var result = _data.Values.Where(s => s.AcademicYear != null && s.AcademicYear.Id == academicYearId).ToList();
        return Task.FromResult<IEnumerable<Student>>(result);
    }

    public Task<IEnumerable<Student>> FindByDegreeProgramAsync(Guid degreeProgramId)
    {
        var result = _data.Values.Where(s => s.DegreeProgram != null && s.DegreeProgram.Id == degreeProgramId).Cast<Student>().ToList();
        return Task.FromResult<IEnumerable<Student>>(result);
    }

    public Task<Student> UpdateStatusAsync(Guid studentId, StudentStatus newStatus)
    {
        if (_data.TryGetValue(studentId, out var value) && value is Student student)
        {
            student.Status = newStatus;
            _data[studentId] = student;
            return Task.FromResult(student);
        }
        throw new KeyNotFoundException($"Student with id {studentId} not found.");
    }
}

