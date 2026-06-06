using System;
using System.Threading.Tasks;
using CoreApp.Repositories;

namespace Infrastucture.Repository;

public class MemoryUniversityUnitOfWork : IUniversityUnitOfWork
{
    private readonly IStudentRepository _students;
    private readonly ILecturerRepository _lecturers;
    private readonly IGradeRepository _grades;
    private readonly ICourseRepository _courses;
    private readonly IAcademicYearRepository _academicYears;
    private readonly IDegreeProgramRepository _degreePrograms;

    public MemoryUniversityUnitOfWork(
        IStudentRepository students,
        ILecturerRepository lecturers,
        IGradeRepository grades,
        ICourseRepository courses,
        IAcademicYearRepository academicYears,
        IDegreeProgramRepository degreePrograms
    )
    {
        _students = students;
        _lecturers = lecturers;
        _grades = grades;
        _courses = courses;
        _academicYears = academicYears;
        _degreePrograms = degreePrograms;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public IStudentRepository Students => _students;
    public ILecturerRepository Lecturers => _lecturers;
    public IGradeRepository Grades => _grades;
    public ICourseRepository Courses => _courses;
    public IAcademicYearRepository AcademicYears => _academicYears;
    public IDegreeProgramRepository DegreePrograms => _degreePrograms;

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask;
    }
}

