using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;

namespace Infrastucture.EntityFramework.Repositories;

public class EfLecturerRepository : EfGenericRepository<Lecturer>, ILecturerRepository
{
    public EfLecturerRepository(AppDbContext context) : base(context, context.Set<Lecturer>()) { }

    public async Task<IEnumerable<Lecturer>> FindByCourseAsync(Guid courseId)
    {
        return await _set.Where(l => l.TaughtCorses != null && l.TaughtCorses.Any(c => c.Id == courseId)).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Lecturer>> FindByTitleAsync(string title)
    {
        return await _set.Where(l => l.Title != null && l.Title.Equals(title, StringComparison.OrdinalIgnoreCase)).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Lecturer>> FindByFacultyAsync(string faculty)
    {
        return await _set.Where(l => l.Faculty != null && l.Faculty.Equals(faculty, StringComparison.OrdinalIgnoreCase)).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesByLecturerAsync(Guid lecturerId)
    {
        var lecturer = await _set
            .Include(l => l.TaughtCorses)
            .ThenInclude(c => c.Enrollments)
            .FirstOrDefaultAsync(l => l.Id == lecturerId);

        return lecturer?.TaughtCorses ?? new List<Course>();
    }

    public async Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid lecturerId, Guid courseId)
    {
        var course = await _context.Set<Course>()
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        return course?.Enrollments ?? new List<Student>();
    }

    public async Task<bool> TeachesCourseAsync(Guid lecturerId, Guid courseId)
    {
        return await _context.Set<Course>()
            .AnyAsync(c => c.Id == courseId && c.Lecturer != null && c.Lecturer.Id == lecturerId);
    }
}

