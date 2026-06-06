using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;

namespace Infrastucture.EntityFramework.Repositories;

public class EfGradeRepository : EfGenericRepository<Grade>, IGradeRepository
{
    public EfGradeRepository(AppDbContext context) : base(context, context.Set<Grade>()) { }

    public async Task<IEnumerable<Grade>> FindByCourseAsync(Guid courseId)
    {
        return await _set.Where(g => g.Course != null && g.Course.Id == courseId).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Grade>> FindByStudentAsync(Guid studentId)
    {
        return await _set.Where(g => g.Student != null && g.Student.Id == studentId).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Grade>> FindByAcademicYearAsync(Guid academicYearId)
    {
        return await _set.Where(g => g.AcademicYear != null && g.AcademicYear.Id == academicYearId).AsNoTracking().ToListAsync();
    }

    public async Task<double?> GetAverageForStudentAsync(Guid studentId)
    {
        var grades = await _set.Where(g => g.Student != null && g.Student.Id == studentId).ToListAsync();
        if (!grades.Any()) return null;
        var avg = grades.Average(g => (double)g.GradeValue);
        return avg;
    }

    public async Task<IEnumerable<Grade>> FindByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _set
            .Where(g => g.Student != null && g.Student.Id == studentId &&
                       g.Course != null && g.Course.Id == courseId)
            .Include(g => g.ChangeHistory)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddGradeChangeAsync(Guid gradeId, GradeValue? previousValue, GradeValue newValue, string changedBy)
    {
        var changeHistory = new GradeChangeHistory
        {
            Id = Guid.NewGuid(),
            GradeId = gradeId,
            PreviousValue = previousValue,
            NewValue = newValue,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow
        };

        await _context.Set<GradeChangeHistory>().AddAsync(changeHistory);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GradeChangeHistory>> GetChangeHistoryAsync(Guid gradeId)
    {
        return await _context.Set<GradeChangeHistory>()
            .Where(h => h.GradeId == gradeId)
            .OrderBy(h => h.ChangedAt)
            .AsNoTracking()
            .ToListAsync();
    }
}

