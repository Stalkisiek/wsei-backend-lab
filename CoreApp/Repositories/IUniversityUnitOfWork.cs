using System;
using System.Threading.Tasks;

namespace CoreApp.Repositories;

public interface IUniversityUnitOfWork : IAsyncDisposable
{
    IStudentRepository Students { get; }
    ILecturerRepository Lecturers { get; }
    IGradeRepository Grades { get; }
    ICourseRepository Courses { get; }
    IAcademicYearRepository AcademicYears { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

