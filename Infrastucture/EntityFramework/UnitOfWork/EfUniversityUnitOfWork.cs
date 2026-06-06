using System;
using System.Threading.Tasks;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;
using Infrastucture.EntityFramework.Repositories;

namespace Infrastucture.EntityFramework.UnitOfWork;

public class EfUniversityUnitOfWork : IUniversityUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly EfStudentRepository _students;
    private readonly EfLecturerRepository _lecturers;
    private readonly EfGradeRepository _grades;
    private readonly EfCourseRepository _courses;
    private readonly EfAcademicYearRepository _years;
    private readonly EfDegreeProgramRepository _degreePrograms;

    public EfUniversityUnitOfWork(
        EfStudentRepository students,
        EfLecturerRepository lecturers,
        EfGradeRepository grades,
        EfCourseRepository courses,
        EfAcademicYearRepository years,
        EfDegreeProgramRepository degreePrograms,
        AppDbContext context)
    {
        _students = students ?? throw new ArgumentNullException(nameof(students));
        _lecturers = lecturers ?? throw new ArgumentNullException(nameof(lecturers));
        _grades = grades ?? throw new ArgumentNullException(nameof(grades));
        _courses = courses ?? throw new ArgumentNullException(nameof(courses));
        _years = years ?? throw new ArgumentNullException(nameof(years));
        _degreePrograms = degreePrograms ?? throw new ArgumentNullException(nameof(degreePrograms));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IStudentRepository Students => _students;
    public ILecturerRepository Lecturers => _lecturers;
    public IGradeRepository Grades => _grades;
    public ICourseRepository Courses => _courses;
    public IAcademicYearRepository AcademicYears => _years;
    public IDegreeProgramRepository DegreePrograms => _degreePrograms;

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }

    public Task CommitTransactionAsync()
    {
        return _context.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync()
    {
        return _context.Database.RollbackTransactionAsync();
    }
}

