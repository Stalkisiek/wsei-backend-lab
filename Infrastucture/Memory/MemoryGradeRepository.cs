using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryGradeRepository : MemoryGenericRepository<Grade>, IGradeRepository
{
    private static readonly Dictionary<Guid, List<GradeChangeHistory>> ChangeHistoryStorage = new();

    public Task<IEnumerable<Grade>> FindByCourseAsync(Guid courseId)
    {
        var result = _data.Values.Where(g => g.Course != null && g.Course.Id == courseId).ToList();
        return Task.FromResult<IEnumerable<Grade>>(result);
    }

    public Task<IEnumerable<Grade>> FindByStudentAsync(Guid studentId)
    {
        var result = _data.Values.Where(g => g.Student != null && g.Student.Id == studentId).ToList();
        return Task.FromResult<IEnumerable<Grade>>(result);
    }

    public Task<IEnumerable<Grade>> FindByAcademicYearAsync(Guid academicYearId)
    {
        var result = _data.Values.Where(g => g.AcademicYear != null && g.AcademicYear.Id == academicYearId).ToList();
        return Task.FromResult<IEnumerable<Grade>>(result);
    }

    public Task<double?> GetAverageForStudentAsync(Guid studentId)
    {
        var grades = _data.Values.Where(g => g.Student != null && g.Student.Id == studentId).ToList();
        if (!grades.Any()) return Task.FromResult<double?>(null);
        var avg = grades.Average(g => (double)g.GradeValue);
        return Task.FromResult<double?>(avg);
    }

    public Task<IEnumerable<Grade>> FindByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        var result = _data.Values
            .Where(g => g.Student != null && g.Student.Id == studentId &&
                       g.Course != null && g.Course.Id == courseId)
            .ToList();
        return Task.FromResult<IEnumerable<Grade>>(result);
    }

    public Task AddGradeChangeAsync(Guid gradeId, GradeValue? previousValue, GradeValue newValue, string changedBy)
    {
        if (!ChangeHistoryStorage.ContainsKey(gradeId))
            ChangeHistoryStorage[gradeId] = new List<GradeChangeHistory>();

        var change = new GradeChangeHistory
        {
            Id = Guid.NewGuid(),
            GradeId = gradeId,
            PreviousValue = previousValue,
            NewValue = newValue,
            ChangedBy = changedBy,
            ChangedAt = DateTime.UtcNow
        };

        ChangeHistoryStorage[gradeId].Add(change);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<GradeChangeHistory>> GetChangeHistoryAsync(Guid gradeId)
    {
        if (ChangeHistoryStorage.TryGetValue(gradeId, out var history))
            return Task.FromResult<IEnumerable<GradeChangeHistory>>(history.OrderBy(h => h.ChangedAt));
        
        return Task.FromResult<IEnumerable<GradeChangeHistory>>(new List<GradeChangeHistory>());
    }
}

