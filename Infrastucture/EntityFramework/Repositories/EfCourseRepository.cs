using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoreApp.Models;
using CoreApp.Repositories;
using Infrastucture.EntityFramework.Context;

namespace Infrastucture.EntityFramework.Repositories;

public class EfCourseRepository : EfGenericRepository<Course>, ICourseRepository
{
    public EfCourseRepository(AppDbContext context) : base(context, context.Set<Course>()) { }

    public async Task<IEnumerable<Course>> FindByDegreeProgramAsync(Guid degreeProgramId)
    {
        return await _set.Where(c => c.DegreeProgram != null && c.DegreeProgram.Id == degreeProgramId).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Course>> FindByAcademicYearAsync(Guid academicYearId)
    {
        return await _set.Where(c => c.AcademicYear != null && c.AcademicYear.Id == academicYearId).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Course>> FindByLecturerAsync(Guid lecturerId)
    {
        return await _set.Where(c => c.Enrollments != null && c.Enrollments.Any(s => s.StudentId == lecturerId) == false).AsNoTracking().ToListAsync();
    }

    public async Task<Course?> FindByCodeAsync(string code)
    {
        return await _set.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code);
    }
}

