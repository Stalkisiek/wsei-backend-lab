using System;
using System.Threading.Tasks;
using CoreApp.Repositories;

namespace Infrastucture.Repository;

public class MemoryUniversityUnitOfWork : IUniversityUnitOfWork
{
    private readonly IStudentRepository _students;
    private readonly ILecturerRepository _lecturers;
    private readonly IGradeRepository _grades;

    public MemoryUniversityUnitOfWork(
        IStudentRepository students,
        ILecturerRepository lecturers,
        IGradeRepository grades
    )
    {
        _students = students;
        _lecturers = lecturers;
        _grades = grades;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public IStudentRepository Students => _students;
    public ILecturerRepository Lecturers => _lecturers;
    public IGradeRepository Grades => _grades;

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

